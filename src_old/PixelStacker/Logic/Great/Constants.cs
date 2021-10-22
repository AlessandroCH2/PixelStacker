﻿namespace PixelStacker.Logic
{
    public static class Constants
    {
        public const string Version = "1.17.1c";
        public const string Website = "https://taylorlove.info/pixelstacker";
        public const int TextureSize = 16;
        public const string RenderedZIndexFilter = "RenderedZIndexFilter";
        public const string ERR_DownsizeYourImage = "Your image is too large and cannot be processed. Please downsize your image using the sizing options or else choose a different image.";
        public const int MAX_HISTORY_SIZE = 20;
        public const float MAX_ZOOM = 200F;
        public const float MIN_ZOOM = 200F;
        public const int SMALL_IMAGE_DIVIDE_SIZE = 2;
        public const int WORLD_HEIGHT = 1024;
        public const int BIG_IMG_MAX_AREA_B4_SPLIT = 100000; // split image into smaller sizes if bigger than this

        // Data version of MC build. Can be found on wiki.
        // TODO: Auto calc this based on selected materials
        // https://minecraft.fandom.com/wiki/Java_Edition_1.17.1
        public const int DataVersion = 2730;

#if FULL_VERSION
        public const bool IsFullVersion = true;
#else
        public const bool IsFullVersion = false;
#endif
    }
}
