namespace Migrations
open SimpleMigrations

[<Migration(202201190806L, "Create Users")>]
type CreateUsers() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Users(
      id TEXT NOT NULL,
      email TEXT NOT NULL,
      password TEXT NOT NULL,
      is_admin BOOLEAN NOT NULL,
      created_on DATETIME NOT NULL,
      updated_on DATETIME NOT NULL
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Users")