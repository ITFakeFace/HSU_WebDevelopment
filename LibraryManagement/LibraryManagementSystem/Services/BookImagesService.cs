namespace LibraryManagementSystem.Services
{
    public class BookImagesService
    {
        private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public async Task<List<byte[]?>> ProcessBookImagesAsync(List<IFormFile> bookImages)
        {
            List<byte[]?> processedImages = new List<byte[]?>();

            foreach (var bookImage in bookImages)
            {
                if (bookImage != null && bookImage.Length > 0)
                {
                    var fileExtension = Path.GetExtension(bookImage.FileName).ToLower();

                    if (!_allowedExtensions.Contains(fileExtension))
                    {
                        throw new InvalidOperationException($"File extension '{fileExtension}' is not allowed. Only JPG, PNG, and GIF are supported.");
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await bookImage.CopyToAsync(memoryStream);
                        processedImages.Add(memoryStream.ToArray());
                    }
                }
            }

            return processedImages;
        }
    }
}
