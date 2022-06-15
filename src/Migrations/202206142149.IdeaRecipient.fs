namespace Migrations
open SimpleMigrations

[<Migration(202206142149L, "Create IdeaRecipients")>]
type CreateIdeaRecipients() =
  inherit Migration()

  override __.Up() =
    base.Execute(@"CREATE TABLE IdeaRecipients(
      idea_id UUID NOT NULL,
      recipient_id UUID NOT NULL,

      PRIMARY KEY (idea_id, recipient_id),
      FOREIGN KEY (idea_id) REFERENCES ideas (id),
      FOREIGN KEY (recipient_id) REFERENCES recipients (id)
    )")

  override __.Down() =
    base.Execute(@"DROP TABLE IdeaRecipients")
