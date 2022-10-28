module IdeasRepository

open Expecto
open System
open Ideas
open Ideas.Repository
open Recipients

[<Tests>]
let tests = testList "Idea database model mapping tests" [
    testList "Idea data model to domain model tests" [
        let ideaDb =
            { id = Guid.NewGuid ()
              user_id = Guid.NewGuid ()
              text = "A brand-new pair of SandwichPants(tm)"
              price = Nullable<decimal> 46.99m
              link = "https://sandwichpants.fun"
              created_on = DateTime.Now
              updated_on = DateTime.Now.AddDays 2 }

        testCase "Id maps to domain Id correctly" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.equal idea.Id ideaDb.id "Ids are the same"

        testCase "User_id maps to domain UserId correctly" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.equal idea.UserId ideaDb.user_id "User ids are the same"

        testCase "Text maps to text correctly" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.equal idea.Text ideaDb.text "Text is the same"

        testCase "Price maps to Some if exists" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.isSome idea.Price "Price exists"
            Expect.equal idea.Price.Value ideaDb.price.Value "Prices are the same"

        testCase "Price maps to None if null" <| fun _ ->
            let idea = toIdea { ideaDb with price = System.Nullable<decimal>() }
            Expect.isNone idea.Price "Price is none"

        testCase "Link maps to Some if exists" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.isSome idea.Link "Link exists"
            Expect.equal idea.Link.Value ideaDb.link "Links are the same"

        testCase "Link maps to None if null" <| fun _ ->
            let idea = toIdea { ideaDb with link = null }
            Expect.isNone idea.Link "Link is none"

        testCase "Created_on maps to domain CreatedOn" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.equal idea.CreatedOn ideaDb.created_on "Created dates match"

        testCase "Updated_on maps to domain UpdatedOn" <| fun _ ->
            let idea = toIdea ideaDb
            Expect.equal idea.UpdatedOn ideaDb.updated_on "Updated dates match"
    ]

    testList "Idea domain model to data model tests" [
        let idea =
            { Id = Guid.NewGuid()
              UserId = Guid.NewGuid ()
              Recipient = []
              Text = "A new hirto-undicovered variety of cabbage"
              Price = Some 1.99m
              Link = Some "https://my-cabbages.atla"
              CreatedOn = DateTime.Now
              UpdatedOn = DateTime.Now }

        testCase "Id maps to data Id correctly" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.equal ideaDb.id idea.Id "Ids are the same"

        testCase "UserId maps to data User_id correctly" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.equal ideaDb.user_id idea.UserId "User ids are the same"

        testCase "Text maps to data text correctly" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.equal ideaDb.text idea.Text "Text is the same"

        testCase "Price maps to value if Some" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.isFalse (box ideaDb.price = null) "Price is not null"
            Expect.equal ideaDb.price.Value idea.Price.Value "Prices are the same"

        testCase "Price maps to null if None" <| fun _ ->
            let ideaDb = fromIdea { idea with Price = None }
            Expect.isTrue (box ideaDb.price = null) "Price is null"

        testCase "Link maps to value if Some" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.isSome idea.Link "Link exists"
            Expect.equal idea.Link.Value ideaDb.link "Links are the same"

        testCase "Link maps to null if None" <| fun _ ->
            let ideaDb = fromIdea { idea with Link = None }
            Expect.isNull ideaDb.link "Link is none"

        testCase "CreatedOn maps to data Created_on" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.equal idea.CreatedOn ideaDb.created_on "Created dates match"

        testCase "UpdatedOn maps to domain Updated_on" <| fun _ ->
            let ideaDb = fromIdea idea
            Expect.equal idea.UpdatedOn ideaDb.updated_on "Updated dates match"
    ]    
]