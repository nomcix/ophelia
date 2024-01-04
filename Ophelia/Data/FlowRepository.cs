using System.Data;
using Npgsql;
using Dapper;
using Ophelia.Data.Models;

namespace Ophelia.Data;

public class FlowRepository : IFlowRepository
{
    private readonly string _connectionString;

    public FlowRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("OpheliaDatabase");
    }

    public async Task AddFlow(Flow flow)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync(); // Explicitly open the connection

        await using var transaction = await connection.BeginTransactionAsync(); // Start the transaction

        try
        {
            await connection.ExecuteAsync("public.insert_flow", new
            {
                flow_id = flow.id,
                flow_title = flow.title,
                flow_category = flow.category,
                flow_system = flow.system
            }, commandType: CommandType.StoredProcedure, transaction: transaction);
            
            foreach (var affirmationId in flow.affirmation_ids)
            {
                await connection.ExecuteAsync("public.insert_flow_affirmation", new
                {
                    flow_id = flow.id,
                    affirmation_id = affirmationId
                }, commandType: CommandType.StoredProcedure, transaction: transaction);
            }

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<Flow> GetFlow(string flowId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var query = "SELECT * FROM public.get_flow_by_id(@p_flow_id)";
        return (await connection.QueryAsync<Flow>(query, new
        {
            p_flow_id = flowId
        })).FirstOrDefault() ?? throw new InvalidOperationException(); // handle null ?
    }

    public async Task<List<UserFlow>> GetAllFlowsByUser(string userId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        var query = "SELECT * FROM public.get_user_flow_likes(@p_user_id);";
        return (await connection.QueryAsync<UserFlow>(query, new
        {
            p_user_id = new Guid(userId)
        })).ToList();
    }


    public async Task AssociateFlowWithUser(string userId, string flowId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("public.insert_user_flow_like", new
        {
            user_id = new Guid(userId),
            flow_id = flowId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task DissociateFlowFromUser(string userId, string flowId)
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.ExecuteAsync("public.delete_user_flow_like", new
        {
            p_user_id = new Guid(userId),
            p_flow_id = flowId
        }, commandType: CommandType.StoredProcedure);
    }
}
