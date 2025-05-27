using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF_Config.Models;
using Microsoft.EntityFrameworkCore;    

namespace WPF_Config.Data
{
    //Instalar lo siguiente
    public class DBConfig : DbContext
    {
        public DbSet<UrlConfig> UrlConfig { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=DESKTOP-JMH\\SQLEXPRESS;Initial Catalog=ApiWhatsAppConfig;Integrated Security=True;Connect Timeout=3;TrustServerCertificate=True;\r\n");
        }
    }
}