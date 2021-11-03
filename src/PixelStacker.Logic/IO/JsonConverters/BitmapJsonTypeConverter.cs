﻿//using Newtonsoft.Json;
//using System;
//using System.Drawing;
//using PixelStacker.Extensions;
//using System.IO;
//using System.Drawing.Imaging;

//namespace PixelStacker.Logic.IO.JsonConverters
//{
//    class BitmapJsonTypeConverter : JsonConverter<Bitmap>
//    {
//        public override Bitmap ReadJson(JsonReader reader, Type objectType, Bitmap existingValue, bool hasExistingValue, JsonSerializer serializer)
//        {
//            if (reader.TokenType != JsonToken.String) return default;
//            string b64 = reader.Value as string;
//            if (string.IsNullOrEmpty(b64)) return default;
//            byte[] bytes = Convert.FromBase64String(b64);
//            using (MemoryStream ms = new MemoryStream())
//            {
//                ms.Write(bytes, 0, bytes.Length);
//                Bitmap rt = new Bitmap(System.Drawing.Image.FromStream(ms));
//                return rt;
//            }
//        }

//        public override void WriteJson(JsonWriter writer, Bitmap value, JsonSerializer serializer)
//        {
//            var bm = value.To32bppBitmap();
//            using (MemoryStream ms = new MemoryStream())
//            {
//                bm.Save(ms, ImageFormat.Png);
//                byte[] byteImage = ms.ToArray();
//                string b64 = Convert.ToBase64String(byteImage); // Get Base64
//                writer.WriteValue(b64);
//            }
//        }
//    }
//}
