namespace Migrations
open SimpleMigrations

[<Migration(202201190806L, "Create Users")>]
type CreateUsers() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Users(
      id UUID NOT NULL,
      email TEXT NOT NULL,
      password TEXT NOT NULL,
      is_admin BOOLEAN NOT NULL,
      created_on TIMESTAMP NOT NULL,
      updated_on TIMESTAMP NOT NULL,

      PRIMARY KEY (id)
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Users")
