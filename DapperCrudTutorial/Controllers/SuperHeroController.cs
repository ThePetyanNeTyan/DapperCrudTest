using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace DapperCrudTutorial.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuperHeroController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SuperHeroController(IConfiguration config)
        {
            _configuration = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<SuperHero>>> GetAllSuperHeroes()
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var heroes = await connection.QueryAsync<SuperHero>("select * from public.\"SuperHeroes\"");
            return Ok(heroes);
        }

        [HttpGet("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> GetHero(int heroId)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            var hero = await connection.QueryFirstAsync<SuperHero>("select * from public.\"SuperHeroes\" where \"Id\" = @Id",
                new { Id = heroId});
            return Ok(hero);
        }

        [HttpPost]
        public async Task<ActionResult<List<SuperHero>>> CreateHero(SuperHero hero)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into public.\"SuperHeroes\" (\"Name\", \"FirstName\", \"LastName\", \"Place\")" +
                                            "values (@Name, @FirstName, @LastName, @Place)", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        private static async Task<IEnumerable<SuperHero>> SelectAllHeroes(NpgsqlConnection connection)
        {
            return await connection.QueryAsync<SuperHero>("select * from public.\"SuperHeroes\"");
        }

        [HttpPut]
        public async Task<ActionResult<List<SuperHero>>> UpdateHero(SuperHero hero)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update public.\"SuperHeroes\" set \"Name\" = @Name, \"FirstName\" = @FirstName, " +
                                            "\"LastName\" = @LastName, \"Place\" = @Place where \"Id\" = @Id", hero);
            return Ok(await SelectAllHeroes(connection));
        }

        [HttpDelete("{heroId}")]
        public async Task<ActionResult<List<SuperHero>>> DeleteHero(int heroId)
        {
            using var connection = new NpgsqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from public.\"SuperHeroes\" where \"Id\" = @Id", new { Id = heroId});
            return Ok(await SelectAllHeroes(connection));
        }


    }
}
