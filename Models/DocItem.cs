using System.ComponentModel.DataAnnotations;

namespace DocMgmnt.Models
{
    public class DocItem
    {
       
        public IFormFile? File { get; set; }

        public string? PresignedURL { get; set; }

    }
}
