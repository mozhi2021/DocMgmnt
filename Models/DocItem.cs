using System.ComponentModel.DataAnnotations;

namespace DocMgmnt.Models
{
    public class DocItem
    {
        [Key]
        public IFormFile? File { get; set; }

    }
}
