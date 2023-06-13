using System.ComponentModel.DataAnnotations.Schema;

namespace CreateExcelFile.Models
{
    public enum Status
    {
        Creating = 0,
        Completed = 1000
    }

    public class UserFile
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public DateTime? CreatedDate { get; set; }

        [NotMapped]
        public string GetCreatedDate => CreatedDate.HasValue ? CreatedDate.Value.ToShortDateString() : "-";
        
    }
}
