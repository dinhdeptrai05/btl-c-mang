namespace Util
{
    public class Utils
    {

        private class ImgBBData
        {
            [Newtonsoft.Json.JsonProperty("url")]
            public string Url { get; set; }
        }
        private class ImgBBResponse
        {
            [Newtonsoft.Json.JsonProperty("success")]
            public bool Success { get; set; }

            [Newtonsoft.Json.JsonProperty("data")]
            public ImgBBData Data { get; set; }
        }

        public static async Task<string> UploadImageToImgBB(string base64Image)
        {
            try
            {
                // Kiểm tra và xử lý chuỗi base64
                if (string.IsNullOrEmpty(base64Image))
                {
                    return null;
                }

                // Nếu chuỗi base64 có prefix "data:image/...;base64,", cần loại bỏ nó
                if (base64Image.Contains("base64,"))
                {
                    base64Image = base64Image.Split(new[] { "base64," }, StringSplitOptions.None)[1];
                }

                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(base64Image), "image");
                    content.Add(new StringContent("16f3c7c463de293a2a0efebc769de0a7"), "key");

                    var response = await client.PostAsync("https://api.imgbb.com/1/upload", content);
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Lỗi khi upload ảnh: {responseString}");
                        return null;
                    }

                    var jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ImgBBResponse>(responseString);
                    if (jsonResponse?.Success == true && jsonResponse.Data?.Url != null)
                    {
                        return jsonResponse.Data.Url;
                    }
                    else
                    {
                        Console.WriteLine($"Lỗi khi upload ảnh: {responseString}");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi upload ảnh: {ex.Message}");
                return null;
            }
        }
    }

}
