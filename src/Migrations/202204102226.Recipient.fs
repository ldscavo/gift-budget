namespace Migrations
open SimpleMigrations

[<Migration(202204102226L, "Create Recipients")>]
type CreateRecipients() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE Recipients(
      id UUID NOT NULL,
      user_id UUID NOT NULL,
      name TEXT NOT NULL,
      notes TEXT NULL,
      created_on TIMESTAMP NOT NULL,
      updated_on TIMESTAMP NOT NULL,

      PRIMARY KEY (id),
      FOREIGN KEY (user_id) REFERENCES users (id)
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE Recipients")
