namespace TextureSeeker {
    internal class Material {
        public string Name { get; set; }
        public List<Texture> Textures { get; set; }
    }

    internal class Texture {
        public string Type { get; set; }
        public string Path { get; set; }
    }
}
