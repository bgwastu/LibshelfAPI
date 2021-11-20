using System;
using LibshelfAPI.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LibshelfAPI.Test;

public abstract class TestWithSqlite : IDisposable
{
    private const string InMemoryConnectionString = "DataSource=:memory:";
    private readonly SqliteConnection _connection;

    protected readonly LibshelfContext Context;

    protected TestWithSqlite()
    {
        _connection = new SqliteConnection(InMemoryConnectionString);
        _connection.Open();
        var options = new DbContextOptionsBuilder<LibshelfContext>()
            .UseSqlite(_connection)
            .Options;
        Context = new LibshelfContext(options);
        Context.Database.EnsureCreated();

        // Create User
        // Encrypt password with bcrypt
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
        Context.Users.Add(new User
        {
            Id = new Guid(),
            Name = "John Smith",
            Password = hashedPassword,
            Email = "john@smith.com"
        });
        Context.SaveChanges();
    }

    public void Dispose()
    {
        _connection.Close();
    }
}