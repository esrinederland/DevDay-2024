namespace CreateWebMaps.Logic.Data
{
    public class Attachment
    {
        public string? Name { get; set; } = null;

        public string? Base64Data { get; set; } = null;

        public string? Keywords { get; set; } = null;

        public string? ContentType { get; set; } = null;

        /// <summary>
        /// Create a new instance of a attachement, based on a name and a base64 data string.
        /// </summary>
        /// <param name="name">Name of the attachement</param>
        /// <param name="base64Data">Image data as base64 string</param>
        public Attachment(string? name, string? keywords, string? contentType, string? base64Data)
        {
            Name = name;
            Base64Data = base64Data;
            Keywords = keywords;
            ContentType = contentType;
        }
    }
}
