namespace SmartNameplate.Api.DTOs
{
    public class TextTagDto
    {
        public int Id { get; set; }
        public string ElementId { get; set; } = string.Empty;
        public int CardId { get; set; }
        public string TagType { get; set; } = string.Empty;
        public string? CustomLabel { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateTextTagDto
    {
        public string ElementId { get; set; } = string.Empty;
        public int CardId { get; set; }
        public string TagType { get; set; } = string.Empty;
        public string? CustomLabel { get; set; }
        public string? Content { get; set; }
    }

    public class UpdateTextTagDto
    {
        public string? TagType { get; set; }
        public string? CustomLabel { get; set; }
        public string? Content { get; set; }
    }
} 