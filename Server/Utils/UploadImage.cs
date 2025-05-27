using System;
using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Util
{
    public class Utils
    {
        private static Cloudinary GetCloudinaryAccount()
        {
            var account = new Account(
                "dcpw9c6lx",                             // Cloud name
                "943687793161495",                      // API key
                "bgYxq89UEtNcgbGO-GjWoooo7go"            // API secret
            );

            return new Cloudinary(account);
        }

        public static async Task<string> UploadImage(string base64Image)
        {
            try
            {
                if (string.IsNullOrEmpty(base64Image))
                    return null;

                // Loại bỏ prefix nếu có
                if (base64Image.Contains("base64,"))
                    base64Image = base64Image.Split(new[] { "base64," }, StringSplitOptions.None)[1];

                byte[] imageBytes = Convert.FromBase64String(base64Image);
                var cloudinary = GetCloudinaryAccount();

                using (var stream = new MemoryStream(imageBytes))
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription("image.jpg", stream),
                        Folder = "uploads",
                        Transformation = new Transformation()
                            .Crop("limit")
                            .Width(500)
                            .Quality("auto:eco")
                            .FetchFormat("auto")
                    };

                    var uploadResult = await cloudinary.UploadAsync(uploadParams);

                    if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        return uploadResult.SecureUrl.ToString(); // URL đã có transformation
                    }
                    else
                    {
                        Console.WriteLine($"Lỗi upload Cloudinary: {uploadResult.Error?.Message}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi upload Cloudinary: {ex.Message}");
                return null;
            }
        }
    }
}
