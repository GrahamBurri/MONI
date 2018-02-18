namespace BestGirl

open Discord
open Discord.WebSocket
open System
open System.Threading.Tasks
open System.Collections.Generic
open System.IO
open Giraffe.Tasks
open Newtonsoft.Json
open MarkovGenerator

module Extras =
    
    let private rng = new Random()

    let randomElement<'T> (lst : List<'T>) =
        let e = rng.Next (0, lst.Count)
        lst.[e]
    
    // Returns true or false based on specified odds
    let onein x = rng.Next(0, x) = 1

// Right now all it does is record to chain if it's in the right channel
module Monika =

    let _client = new DiscordSocketClient()

    let log (msg : LogMessage) =
        msg.ToString() |> Console.WriteLine
        Task.CompletedTask

    let tagged (msg : SocketMessage) =
        let mutable _tagged = false
        for usr in msg.MentionedUsers do
            if usr.Username = "Monika" || usr.Username = "Monika Dev" then
                _tagged <- true
        _tagged
    
    // let generator = new Markov()
    let generator = new MarkovNextGen();
        
    let messageReceived (msg : SocketMessage) =
        task {
            let author = msg.Author.Username
            let text = msg.Content
            
            // Only process other users' messages
            if author <> "Monika" && author <> "Monika Dev" then
                if tagged msg then
                    if text.Contains " say " then
                        let response = text.Substring (text.IndexOf "say" + 4)
                        let! ignore_response = msg.Channel.SendMessageAsync response
                        () // Do Nothing
                    else if text.Contains " markov " then
                        let ss = text.Substring (text.IndexOf "markov" + 7)
                        if ss |> Seq.forall Char.IsDigit then // If it's a valid number
                            let i = Convert.ToInt32 ss
                            let response = generator.Generate i
                            let! ignore_response = msg.Channel.SendMessageAsync response
                            () // Do Nothing
                    else
                        let line = Lines.VoiceLines |> Extras.randomElement
                        let response = line.Replace("[player]", msg.Author.Mention)
                        let! ignore_response = msg.Channel.SendMessageAsync response
                        () // Do Nothing
                else if text.StartsWith "delete" then
                    let character = text.Substring 7
                    let response = sprintf "%s.chr deleted" character
                    let! ignore_response= msg.Channel.SendMessageAsync response
                    () // Do Nothing
                else if msg.Channel.Name.StartsWith "markov-" then
                    generator.AddToChain text // Record the message
        }
    
    let mainasync = // Might have to eventually pass unit to make this work
        task {

            // Weird type stuff here prevents just saying `_client.add_Log log`
            _client.add_Log (fun m -> log m)
            _client.add_MessageReceived (fun m -> messageReceived m :> Task)
            //_client.add_MessageReceived (fun m -> messageReceived m |> Extras.startAsPlainTask)

            // Tokens
            let DEVELOPMENT = "Dev Token Here"
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