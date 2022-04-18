using System.ComponentModel.DataAnnotations.Schema;

namespace AppLibrary.Models;

public partial class DocumentView :Document{
   
   [NotMapped]
   public string DocumentName { get; set; }

   [NotMapped]
   public bool isValid{ 
      get{
         return ValidThrough>DateTime.Now;
      }
   }
}
