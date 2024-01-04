using System.Data;
using Dapper;
using Npgsql;
using Ophelia.Data.Models;

namespace Ophelia.Data
{
    internal class AffirmationRepository : IAffirmationRepository
    {
        private readonly string _connectionString;

        public AffirmationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("OpheliaDatabase");
        }

        public async Task<IEnumerable<Affirmation>> GetAllAffirmations()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            return await connection.QueryAsync<Affirmation>("SELECT * FROM public.get_all_affirmations();")
                .ConfigureAwait(false);
        }
        
        public async Task<List<Affirmation>> GetAffirmationsByIds(List<string> ids)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM public.get_affirmations_by_ids(@Ids);";
            var result = await connection.QueryAsync<Affirmation>(query, new
                {
                    Ids = ids.ToArray()
                })
                .ConfigureAwait(false);
            return result.ToList();
        }

        public async Task<List<Affirmation>> GetAffirmationsByCategory(string category)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM public.get_affirmations_by_category(@p_category);";
            var result = await connection.QueryAsync<Affirmation>(query, new
                {
                    p_category = category
                })
                .ConfigureAwait(false);
            return result.ToList();
        }
        
        
        public async Task<Affirmation> GetAffirmationById(string id)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM public.get_affirmation_by_id(@Id);";
            return await connection.QueryFirstOrDefaultAsync<Affirmation>(query, new
            {
                Id = id
            }).ConfigureAwait(false) ?? throw new InvalidOperationException();
        }


        public async Task<List<UserAffirmation>> GetAffirmationsByUser(string userId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            var query = "SELECT * FROM public.get_user_likes(@user_id);";
            var result = await connection.QueryAsync<UserAffirmation>(query, new
            {
                user_id = new Guid(userId)
            }).ConfigureAwait(false);
            return result.ToList();
        }
        
        public async Task InsertAffirmations(List<Affirmation> affirmations)
        {
            foreach (var affirmation in affirmations)
            {
                await InsertAffirmation(affirmation).ConfigureAwait(false);
            }
        }

        public async Task InsertAffirmation(Affirmation affirmation)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync("public.insert_affirmation", new
            {
                id = affirmation.id,
                user_id = affirmation.user_id,
                affirmation = affirmation.affirmation,
                category = affirmation.category,
                system = affirmation.system
            }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }
        
        public async Task AssociateAffirmationWithUser(string userId, string affirmationId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync("public.insert_user_like", new
            {
                user_id = new Guid(userId),
                affirmation_id = affirmationId
            }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }

        public async Task DissociateAffirmationFromUser(string userId, string affirmationId)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.ExecuteAsync("public.delete_user_affirmation_like", new
            {
                p_user_id = new Guid(userId),
                p_affirmation_id = affirmationId
            }, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        }
    }
}