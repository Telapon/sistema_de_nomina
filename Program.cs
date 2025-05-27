// Sistema de Gestión de Pagos de Nómina
using System;
using System.Collections.Generic;
using System.Linq;

namespace SistemaNomina
{
    // Clase base para empleados
    abstract class Empleado
    {
        public string PrimerNombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string NumeroSeguroSocial { get; set; }

        public Empleado(string primerNombre, string apellidoPaterno, string nss)
        {
            PrimerNombre = primerNombre;
            ApellidoPaterno = apellidoPaterno;
            NumeroSeguroSocial = nss;
        }

        public abstract decimal CalcularPago();
        public abstract string ObtenerDetallePago();
    }

    class EmpleadoAsalariado : Empleado
    {
        public decimal SalarioSemanal { get; set; }

        public EmpleadoAsalariado(string primerNombre, string apellidoPaterno, string nss, decimal salarioSemanal)
            : base(primerNombre, apellidoPaterno, nss)
        {
            SalarioSemanal = salarioSemanal;
        }

        public override decimal CalcularPago() => SalarioSemanal;
        public override string ObtenerDetallePago() => $"Pago fijo semanal: {SalarioSemanal:C}";
    }

    class EmpleadoPorHoras : Empleado
    {
        public decimal SueldoPorHora { get; set; }
        public decimal HorasTrabajadas { get; set; }

        public EmpleadoPorHoras(string apellidoPaterno, string nss, decimal sueldoPorHora, decimal horasTrabajadas)
            : base("", apellidoPaterno, nss)
        {
            SueldoPorHora = sueldoPorHora;
            HorasTrabajadas = horasTrabajadas;
        }

        public override decimal CalcularPago()
        {
            if (HorasTrabajadas <= 40)
                return SueldoPorHora * HorasTrabajadas;
            else
                return (SueldoPorHora * 40) + (SueldoPorHora * 1.5m * (HorasTrabajadas - 40));
        }

        public override string ObtenerDetallePago()
        {
            if (HorasTrabajadas <= 40)
                return $"{HorasTrabajadas} horas a {SueldoPorHora:C}/hora = {CalcularPago():C}";
            else
                return $"40h normales + {(HorasTrabajadas - 40)}h extra = {CalcularPago():C}";
        }
    }

    class EmpleadoPorComision : Empleado
    {
        public decimal VentasBrutas { get; set; }
        public decimal TarifaComision { get; set; }

        public EmpleadoPorComision(string primerNombre, string apellidoPaterno, string nss, decimal ventasBrutas, decimal tarifaComision)
            : base(primerNombre, apellidoPaterno, nss)
        {
            VentasBrutas = ventasBrutas;
            TarifaComision = tarifaComision;
        }

        public override decimal CalcularPago() => VentasBrutas * TarifaComision;
        public override string ObtenerDetallePago() => $"Comisión de {TarifaComision:P} sobre {VentasBrutas:C} = {CalcularPago():C}";
    }

    class EmpleadoAsalariadoPorComision : EmpleadoPorComision
    {
        public decimal SalarioBase { get; set; }

        public EmpleadoAsalariadoPorComision(string primerNombre, string apellidoPaterno, string nss, decimal ventasBrutas, decimal tarifaComision, decimal salarioBase)
            : base(primerNombre, apellidoPaterno, nss, ventasBrutas, tarifaComision)
        {
            SalarioBase = salarioBase;
        }

        public override decimal CalcularPago() => base.CalcularPago() + SalarioBase + (SalarioBase * 0.10m);
        public override string ObtenerDetallePago()
        {
            return $"Base: {SalarioBase:C} + Bono: {(SalarioBase * 0.10m):C} + Comisión: {base.CalcularPago():C} = {CalcularPago():C}";
        }
    }

    class SistemaNomina
    {
        private List<Empleado> empleados = new List<Empleado>();

        public void AgregarEmpleado(Empleado empleado) => empleados.Add(empleado);

        public void ActualizarEmpleado(string nss, Action<Empleado> accion)
        {
            var empleado = empleados.FirstOrDefault(e => e.NumeroSeguroSocial == nss);
            if (empleado != null)
            {
                accion(empleado);
            }
        }

        public void GenerarReporteSemanal()
        {
            Console.WriteLine("--- Reporte Semanal de Nómina ---");
            foreach (var emp in empleados)
            {
                Console.WriteLine($"Empleado: {emp.PrimerNombre} {emp.ApellidoPaterno} | NSS: {emp.NumeroSeguroSocial}");
                Console.WriteLine(emp.ObtenerDetallePago());
                Console.WriteLine("-----------------------------");
            }
        }
    }

    class Program
    {
        static void Main()
        {
            var sistema = new SistemaNomina();

            sistema.AgregarEmpleado(new EmpleadoAsalariado("Ana", "Pérez", "123", 1000));
            sistema.AgregarEmpleado(new EmpleadoPorHoras("López", "456", 100, 45));
            sistema.AgregarEmpleado(new EmpleadoPorComision("Carlos", "Ramírez", "789", 20000, 0.05m));
            sistema.AgregarEmpleado(new EmpleadoAsalariadoPorComision("Luis", "Fernández", "321", 15000, 0.04m, 800));

            sistema.GenerarReporteSemanal();
        }
    }
}
