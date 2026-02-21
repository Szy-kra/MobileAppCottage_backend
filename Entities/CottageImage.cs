using System.ComponentModel.DataAnnotations;

namespace CottageAPI.Entities
{
    public class CottageImage
    {
        [Key]
        public int Id { get; set; }

        public string Url { get; set; } = string.Empty;

        public int CottageId { get; set; }

        public virtual Cottage Cottage { get; set; } = null!;
    }
}