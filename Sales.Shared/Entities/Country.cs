using System.ComponentModel.DataAnnotations;

namespace Sales.Shared.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Pais")]
        [Required (ErrorMessage =  "El campo {0} es obligatorio.")]
        [MaxLength(100, ErrorMessage = "El campo{0} no puede tener {1} caracteres")]
        public string Name { get; set; } = null!;

        public ICollection<State>? States { get; set; }
        [Display(Name = "Estados/Departamentos123")]
        public int StatesNumber => States == null ? 0 : States.Count;

    }
}
