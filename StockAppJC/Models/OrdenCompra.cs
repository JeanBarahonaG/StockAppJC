using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StockAppJC.Models
{
    public class OrdenCompra
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }

        // Relación con Usuario (Identity)
        public string UsuarioId { get; set; }
        public Usuario Usuario { get; set; }  // El usuario que realizó la orden

        public ICollection<OrdenCompraDetalle> Detalles { get; set; }  // Relación con detalles de la compra
    }
}

