using Microsoft.AspNetCore.Mvc;

namespace MiApiEDO.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculadoraController : ControllerBase
    {
        public class PeticionEDO
        {
            public double A { get; set; }
            public double B { get; set; }
            public double C { get; set; }
            public double F { get; set; }
            public double X0 { get; set; }
            public double V0 { get; set; }
            public double Tf { get; set; }
            public int Pasos { get; set; }
            public bool SegundoOrden { get; set; }
        }

        public class Dato
        {
            public double Tiempo { get; set; }
            public double Valor { get; set; }
        }

        [HttpPost("resolver")]
        public IActionResult Resolver([FromBody] PeticionEDO req)
        {
            try
            {
                if (req.A == 0) return BadRequest(new { mensaje = "El coeficiente a debe ser diferente de cero." });
                if (req.Tf <= 0) return BadRequest(new { mensaje = "El tiempo final debe ser mayor que cero." });
                if (req.Pasos < 10 || req.Pasos > 50000) return BadRequest(new { mensaje = "El número de pasos debe estar entre 10 y 50000." });

                Func<double[], double[]> derivadas;
                double[] inicial;

                if (req.SegundoOrden)
                {
                    derivadas = y => new double[] { y[1], (req.F - req.B * y[1] - req.C * y[0]) / req.A };
                    inicial = new double[] { req.X0, req.V0 };
                }
                else
                {
                    derivadas = y => new double[] { (req.F - req.B * y[0]) / req.A };
                    inicial = new double[] { req.X0 };
                }

                List<Dato> datos = Integrar(derivadas, inicial, req.Tf, req.Pasos);
                return Ok(datos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        private double[] Sumar(double[] y, double[] incremento, double factor)
        {
            double[] resultado = new double[y.Length];
            for (int j = 0; j < y.Length; j++) resultado[j] = y[j] + factor * incremento[j];
            return resultado;
        }

        private void Verificar(double[] valores)
        {
            foreach (double valor in valores)
            {
                if (double.IsNaN(valor) || double.IsInfinity(valor) || Math.Abs(valor) > 1e12)
                    throw new Exception("El cálculo supera el rango admitido o pierde estabilidad.");
            }
        }

        private List<Dato> Integrar(Func<double[], double[]> derivadas, double[] inicial, double tf, int pasos)
        {
            double h = tf / pasos;
            double[] y = (double[])inicial.Clone();
            List<Dato> datos = new List<Dato> { new Dato { Tiempo = 0, Valor = y[0] } };

            for (int i = 0; i < pasos; i++)
            {
                double[] k1 = derivadas(y);
                double[] k2 = derivadas(Sumar(y, k1, h / 2));
                double[] k3 = derivadas(Sumar(y, k2, h / 2));
                double[] k4 = derivadas(Sumar(y, k3, h));

                double[] siguiente = new double[y.Length];
                for (int j = 0; j < y.Length; j++)
                    siguiente[j] = y[j] + h / 6 * (k1[j] + 2 * k2[j] + 2 * k3[j] + k4[j]);

                Verificar(siguiente);
                y = siguiente;
                datos.Add(new Dato { Tiempo = (i + 1) * h, Valor = y[0] });
            }
            return datos;
        }
    }
}
