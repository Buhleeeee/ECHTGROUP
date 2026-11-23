using Microsoft.AspNetCore.Http;

namespace ECHTGROUP.Models
{ //start of namespace

    public static class PhotoStore
    { //start of class
        private static readonly string[] Allowed = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long MaxBytes = 2 * 1024 * 1024;

        public static async Task<string?> SaveAsync(IFormFile file, string webRoot, string folder, string prefix)
        { //start of method
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!Allowed.Contains(ext) || file.Length < 12 || file.Length > MaxBytes)
                return null;

            var header = new byte[12];
            using (var stream = file.OpenReadStream())
            {
                await stream.ReadExactlyAsync(header, 0, 12);
            }

            var isJpeg = header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF;
            var isPng = header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47;
            var isWebp = header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46
                      && header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50;
            if (!isJpeg && !isPng && !isWebp) return null;

            var dir = Path.Combine(webRoot, "uploads", folder);
            Directory.CreateDirectory(dir);

            var name = $"{prefix}-{Guid.NewGuid():N}{ext}";
            using (var target = new FileStream(Path.Combine(dir, name), FileMode.Create))
            {
                await file.CopyToAsync(target);
            }

            return $"/uploads/{folder}/{name}";
        } //end of class

        public static void Delete(string? webPath, string webRoot, string folder)
        { //start of method
            var prefix = $"/uploads/{folder}/";
            if (string.IsNullOrEmpty(webPath) || !webPath.StartsWith(prefix)) return;

            var file = Path.Combine(webRoot, "uploads", folder, Path.GetFileName(webPath));
            if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
        } //end of method
    } //end of class
} //end of namespace
