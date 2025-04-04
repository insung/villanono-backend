public class FileUploadModel
{
    public required List<IFormFile> Files { get; set; }
    public required List<string> FileNames { get; set; }
    public required VillanonoDataType DataType { get; set; }
}
