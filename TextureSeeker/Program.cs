using System.Diagnostics;
using System.Text.Json;
using System.Text;

namespace TextureSeeker {
    internal class Program {
        public static Dictionary<string, string> texturesStorage = new Dictionary<string, string>();

        static void Main(string[] args) {
            if (args.Length == 0) {
                Console.WriteLine("drag&drop files here:");
                args = Console.ReadLine().Split(' ');
                Console.Clear();
            }

            var userCfg = JsonSerializer.Deserialize<UserConfig>(
                File.ReadAllText(Path.Combine(AppContext.BaseDirectory,
                    @"userConfig.json")));

            if (userCfg.UnpackTextures)
                UnpackTextures(userCfg);

            foreach (var dcxPath in args) {
                var dcxDir = Path.GetDirectoryName(dcxPath);
                var objectName = Path.GetFileNameWithoutExtension(dcxPath).TrimEnd(".mapbnd".ToCharArray());
                var objectDir = $"{userCfg.ProjectFolder}/{objectName}";
                var allMaterialsDir = $"{objectDir}/materials";

                Directory.CreateDirectory(objectDir);
                Directory.CreateDirectory(allMaterialsDir);

                //dcx to flver
                Convert(userCfg.SoulsModelToolPath, dcxPath);

                //move fbx to project folder
                if (userCfg.GetFbxFile) {
                    var fbxPath = Directory.EnumerateFiles(dcxDir, $"{objectName}.fbx", SearchOption.AllDirectories).FirstOrDefault();
                    File.Copy(fbxPath, Path.Combine(objectDir, Path.GetFileName(fbxPath)), true);
                    File.Delete(fbxPath);
                }

                //get textures paths
                var matData = JsonSerializer.Deserialize<List<Material>>(
                    File.ReadAllText(
                        Directory.EnumerateFiles(dcxDir, $"{objectName}.flver.matData.json", SearchOption.AllDirectories)
                        .FirstOrDefault())
                    );

                foreach (var material in matData) {
                    var materialDir = $"{allMaterialsDir}/{material.Name}";
                    Directory.CreateDirectory(materialDir);

                    //tpf to dds
                    ConvertTextures(userCfg, material, materialDir);

                    //game files to project folder
                    MoveToProject(userCfg, material, materialDir);
                }
            }
        }

        public static void MoveToProject(UserConfig cfg, Material material, string materialDir) {
            var texturesDDS = Directory.EnumerateFiles(cfg.MapTexturesDir, @"*.dds", SearchOption.AllDirectories);
            foreach (var texture in texturesDDS) {
                var newPath = $"{materialDir}/{Path.GetFileName(texture)}";
                var fileName = Path.GetFileNameWithoutExtension(texture);

                if (texturesStorage.ContainsKey(fileName)) {
                    File.Copy(texturesStorage[fileName], newPath, true);
                    continue;
                }

                texturesStorage.Add(Path.GetFileNameWithoutExtension(texture), newPath);

                File.Copy(texture, newPath, true);
                File.Delete(texture);
            }
        }

        public static void ConvertTextures(UserConfig cfg, Material material, string materialDir) {
            StringBuilder sb = new StringBuilder();
            foreach (var texture in material.Textures) {
                var textureName = texture.Path.Split('\\').Where(n => n.EndsWith(".tif")).FirstOrDefault();

                if (textureName == null)
                    continue;

                textureName = textureName.TrimEnd(".tif".ToCharArray());

                if (texturesStorage.ContainsKey(textureName)) {
                    for (int i = 0; i <= 3; i++) {
                        try {
                            File.Copy(texturesStorage[textureName], $"{materialDir}/{textureName}.dds", true);
                            break;
                        }
                        catch (Exception) when (i <= 3) {
                            Thread.Sleep(200);
                        }
                    }
                    continue;
                }
                sb.Append($"\"{Directory.EnumerateFiles(cfg.MapTexturesDir, $"{textureName}.tpf", SearchOption.AllDirectories).FirstOrDefault()}\" ");
            }
            //tpf to dds
            Convert(cfg.SoulsModelToolPath, sb.ToString());
        }

        public static void UnpackTextures(UserConfig cfg) {
            var filesToUnpack = Directory.EnumerateFiles(cfg.MapTexturesDir, "*.tpfbdt", SearchOption.AllDirectories);

            foreach (var file in filesToUnpack) {
                Convert(cfg.BinderToolPath, $"\"{file}\"");
            }
        }

        public static void Convert(string appPath, string appArgs) {
            Process.Start(new ProcessStartInfo {
                FileName = appPath,
                Arguments = appArgs,
                CreateNoWindow = true,
                UseShellExecute = true,
            }).WaitForExit();
        }
    }
}
