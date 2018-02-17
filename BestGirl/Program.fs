namespace BestGirl

open Discord
open Discord.WebSocket
open System
open System.Threading.Tasks
open System.Collections.Generic
open Giraffe.Tasks

module Extras =
    
    let private rng = new Random()

    let randomElement<'T> (lst : List<'T>) =
        let e = rng.Next (0, lst.Count)
        lst.[e]
    
    // Returns true or false based on specified odds
    let onein x = rng.Next(0, x) = 1

    // Runs an async { } workflow as a Task
    // let inline startAsPlainTask (work : Async<unit>) = Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)
   

module Monika =
    
    let (|Say|Monika|Delete|Other|) (txt : string) =
        if txt.ToLower().Contains "<@407367655830585344> say" then Say
        else if txt.ToLower().Contains "<@407367655830585344>" then Monika
        else if txt.ToLower().StartsWith "delete" then Delete
        else Other

    let _client = new DiscordSocketClient()

    let log (msg : LogMessage) =
        msg.ToString() |> Console.WriteLine
        Task.CompletedTask
    
    let messageReceived (msg : SocketMessage) =
        task {
            let author = msg.Author.Username
            let text = msg.Content
            if text.StartsWith ";" then ()
            match msg.Author.Username with
                | "Monika" -> ()
                | usr ->
                    match text with
                        | Say ->
                            let monikasay = "<@407367655830585344> say"
                            let cutoff = text.IndexOf monikasay + monikasay.Length + 1
                            let response = text.Substring cutoff
                            let! throwaway_response = msg.Channel.SendMessageAsync response
                            () // return unit
                        | Monika ->
                            let line = Lines.VoiceLines |> Extras.randomElement
                            let response = line.Replace("[player]", msg.Author.Mention)
                            let! throwaway_response = msg.Channel.SendMessageAsync response
                            () // return unit
                        | Delete ->
                            let response = text.Substring 7 + ".chr deleted"
                            let! throwaway_response = msg.Channel.SendMessageAsync response
                            () // return unit
                        | Other -> // Message doesn't match any command
                            if Extras.onein 10 then // One in ten chance of saying a voice line
                                if Extras.onein 2 then // One in two chance of saying a preset line
                                    let line = Lines.VoiceLines |> Extras.randomElement
                                    let response = line.Replace("[player]", msg.Author.Mention)
                                    let! throwaway_response = msg.Channel.SendMessageAsync response
                                    () // return unit
                                else // One in two chance of saying a Markov generated line
                                    let line = Lines.VoiceLines |> Extras.randomElement
                                    let response = line.Replace("[player]", msg.Author.Mention)
                                    let! throwaway_response = msg.Channel.SendMessageAsync response
                                    () // return unit
        }
    
    let mainasync = // Might have to eventually pass unit to make this work
        task {

            // Weird type stuff here prevents just saying `_client.add_Log log`
            _client.add_Log (fun m -> log m)
            _client.add_MessageReceived (fun m -> messageReceived m :> Task)
            //_client.add_MessageReceived (fun m -> messageReceived m |> Extras.startAsPlainTask)

            // // https://discordapp.com/oauth2/authorize?client_id=407367655830585344&scope=bot

            // Tokens
            let DEVELOPMENT = "NDA3Mzk4NzkyMzc2MDkwNjI0.DWXvpw.JMkH9omQ8OXzcwpKrbjNRe3IQ8E"
            let RELEASE = "NDA3MzY3NjU1ODMwNTg1MzQ0.DVOrsQ.QHFx1I6h8MTtkJiU_g7G7q0eaQY"
            let token = DEVELOPMENT

            do! _client.LoginAsync (TokenType.Bot, token)
            do! _client.StartAsync ()

            // Keeps it running
            do! Task.Delay -1
        }
        
    [<EntryPoint>]
    let main argv = 
        mainasync.GetAwaiter().GetResult()
        0 // return an integer exit code