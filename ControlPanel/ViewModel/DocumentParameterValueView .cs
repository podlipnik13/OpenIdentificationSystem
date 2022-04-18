using System.ComponentModel.DataAnnotations.Schema;

namespace AppLibrary.Models;

public partial class DocumentParameterValueView  :DocumentParameterValue{
   
   [NotMapped]
   public string ParameterName { get; set; }

}
