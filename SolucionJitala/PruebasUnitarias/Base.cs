using Microsoft.EntityFrameworkCore;
using SolucionJitala.Data;

namespace PruebasUnitarias
{
    public class Base
    {
        protected BaseJitalaContext ConstruirContext(string nombrebd)
        {
            var option = new DbContextOptionsBuilder<BaseJitalaContext>()
                .UseInMemoryDatabase(nombrebd).Options;

            var dbContext = new BaseJitalaContext(option);
            return dbContext;
        }
    }
}
