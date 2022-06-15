namespace Migrations
open SimpleMigrations

[<Migration(202206142142L, "Create Ideas")>]
type Createideas() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE ideas(
      id UUID NOT NULL,
      user_id UUID NOT NULL,
      text TEXT NOT NULL,
      price MONEY NULL,
      link TEXT NULL,
      created_on TIMESTAMP NOT NULL,
      updated_on TIMESTAMP NOT NULL,

      PRIMARY KEY (id),
      FOREIGN KEY (user_id) REFERENCES users (id)
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE ideas")
