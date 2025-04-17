using System.Data;
using Dapper;

namespace MagicalKitties.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using IDbConnection connection = await _dbConnectionFactory.CreateConnectionAsync();

        await connection.ExecuteAsync(new CommandDefinition("""
                                                            insert into account(id, first_name, last_name, username, email, password, created_utc, updated_utc, activated_utc, last_login_utc, deleted_utc, account_status, account_role)
                                                            values('77bd9552-2153-4e72-ad3d-5e0316db6253', 'MagicalKitties', 'Service', 'magicalkittyservice', 'brenton.belanger@gmail.com', '9A003222166F23043D6A97A77AF9D2166838EE3ABDFE65D96169F31E0AD6123E-73506780E1D56922443A9D01BB7D2293', '2025-03-23T09:40:00', '2025-03-23T09:40:00', '2025-03-23T09:40:00', null, null, 1, 0)
                                                            on conflict do nothing
                                                            """));
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            insert into account(id, first_name, last_name, username, email, password, created_utc, updated_utc, last_login_utc, deleted_utc, activated_utc, account_status, account_role)
                                                            values('4174494b-9d60-4d11-bb4a-eff736cc5bf8', 'Brent', 'Belanger', 't3mp0ralflux', 't3mp0ralflux@gmail.com', '9A003222166F23043D6A97A77AF9D2166838EE3ABDFE65D96169F31E0AD6123E-73506780E1D56922443A9D01BB7D2293', '2025-03-23T09:40:00', '2025-03-23T09:40:00', '2025-03-23T09:40:00', null, '2025-03-23T09:05:00', 1, 0)
                                                            on conflict do nothing
                                                            """));
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                            insert into account(id, first_name, last_name, username, email, password, created_utc, updated_utc, last_login_utc, deleted_utc, activated_utc, account_status, account_role)
                                                            values('b95a3fb7-368d-47e2-9296-d6b60e9073b9', 'Big', 'Chungus', 'bigchungus', 'bigchungus@meme.com', '6E008EA9461D8BAB01B70F4F8D50D3242F69C187FB4837594CAE81C9BE647186-9EB5DD5BDC3CDFFAB33DFEA6EDBEDB50', '2025-03-24T09:40:00', '2025-03-24T09:40:00', '2025-03-24T09:40:00', null, '2025-03-23T09:05:00', 0, 1)
                                                            on conflict do nothing
                                                            """));

        await connection.ExecuteAsync(new CommandDefinition("""
                                                             insert into globalsetting(id, name, value)
                                                             values('a8c45217-1a04-4c42-9989-cefb5cdf09a2', 'service_account_username', 'magicalkittyservice')
                                                             on conflict do nothing
                                                            """));
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                             insert into globalsetting(id, name, value)
                                                             values('545c30a9-f23d-4366-9c7f-10d7b24c4700', 'activation_email_format', '<p>Your activation link is:</p><p>{0}</p><p>For security reasons, do not share this link with anyone. This link will stop working after {1} minutes.</p><p>To keep future security messages like this from going to spam or junk email, Add no-reply@mail.magicalkitties.com to your approved or safe sender list.</p><p>If you did not make this request, contact us. Please do not respond to this email.</p>')
                                                             on conflict do nothing
                                                            """));
        
        await connection.ExecuteAsync(new CommandDefinition("""
                                                             insert into globalsetting(id, name, value)
                                                             values('e36716aa-c0a7-49e7-b874-aa7eb52d951d', 'password_reset_email_format', '<p>Your password reset code is:</p><p><b>763242</b></p><p>For security reasons, DO NOT share this with anyone.</p>')
                                                             on conflict do nothing
                                                            """));
    }
}