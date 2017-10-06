module Question.Show.State

open Elmish
open Types
open Okular
open Okular.Operators

let init user id =
    Model.Empty user id , Cmd.ofMsg (GetDetails id)

let verifyReply reply =
    if reply.Value <> "" then
        None
    else
        Some "Your answer can't be empty"

let update msg (model: Model) =
    match msg with
    | GetDetails id ->
        model, Cmd.ofPromise Rest.getDetails id (GetDetailsRes.Success >> GetDetailsResult) (GetDetailsRes.Error >> GetDetailsResult)

    | GetDetailsResult result ->
        match result with
        | GetDetailsRes.Success data ->
            { model with Data = Some data }, Cmd.none

        | GetDetailsRes.Error error ->
            Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
            { model with Data = None }, Cmd.none
    | ChangeReply value ->
        model
        |> Lens.set (Model.ReplyLens >-> StringField.ValueLens) value
        |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) None, Cmd.none

    | Submit ->
        if model.IsWaitingReply then
            model, Cmd.none
        else
            match verifyReply model.Reply with
            | Some msg ->
                model
                |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) (Some msg), Cmd.none
            | None ->
                { model with IsWaitingReply = true }, Cmd.ofPromise
                                                        Rest.createAnswer
                                                        (model.QuestionId, model.Session.Id, model.Reply.Value)
                                                        (CreateAnswerRes.Success >> CreateAnswerResult)
                                                        (CreateAnswerRes.Error >> CreateAnswerResult)
    | CreateAnswerResult result ->
        match result with
        | CreateAnswerRes.Error error ->
            Logger.debugfn "[Question.Show.State] Error when fetching details: \n %A" error
            { model with IsWaitingReply = false }, Cmd.none

        | CreateAnswerRes.Success newAnswer ->
            match model.Data with
            | Some data ->
                { model with IsWaitingReply = false
                             Data = Some { data with Answers = List.append data.Answers [newAnswer] } }
                |> Lens.set (Model.ReplyLens >-> StringField.ValueLens) ""
                |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) None, Cmd.none
            | None ->
                Logger.debug "[Question.Show.State] Can't add answer when data isn't set"
                { model with IsWaitingReply = false }
                |> Lens.set (Model.ReplyLens >-> StringField.ErrorLens) (Some "An error occured please try again, if this error persist contact your admin"), Cmd.none
