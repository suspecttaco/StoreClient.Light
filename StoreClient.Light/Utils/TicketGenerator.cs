using System.Text;
using StoreClient.Light.Models;

namespace StoreClient.Light.Utils;

public class TicketGenerator
{
    public static string GenerateTicketString(Sale venta)
            {
                StringBuilder sb = new StringBuilder();
                
                // Encabezado
                sb.AppendLine("================================");
                sb.AppendLine("            Shopper             ");
                sb.AppendLine("================================");
                sb.AppendLine($"Folio: {venta.Id}");
                sb.AppendLine($"Fecha: {venta.Date:dd/MM/yyyy HH:mm}");
                sb.AppendLine($"Le atendió: {venta.UserName}");
                sb.AppendLine($"Cliente: {venta.CustomerName ?? "Público General"}");
                sb.AppendLine("--------------------------------");
                
                // Productos
                sb.AppendLine(String.Format("{0,-18} {1,5} {2,7}", "Producto", "Cant", "Total"));
                sb.AppendLine("--------------------------------");
    
                foreach (var item in venta.Details)
                {
                    string nombre = item.ProductName.Length > 18 
                        ? item.ProductName.Substring(0, 18) 
                        : item.ProductName;
    
                    sb.AppendLine(String.Format("{0,-18} {1,5} {2,7:0.00}", 
                        nombre, 
                        item.Amount,
                        item.Subtotal));
                }
    
                sb.AppendLine("--------------------------------");
                sb.AppendLine($"TOTAL:          {venta.Total:C2}");
                sb.AppendLine("================================");
                sb.AppendLine("      ¡GRACIAS POR SU COMPRA!   ");
                sb.AppendLine("================================");
    
                return sb.ToString();
            }
}