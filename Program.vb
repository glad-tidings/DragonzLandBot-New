Imports System.IO
Imports System.Text.Json
Imports System.Threading

Module Program
    Private proxies As Proxy()

    Sub Main()
        Console.WriteLine("  ____                                  _                    _ ____   ___ _____ 
 |  _ \ _ __ __ _  __ _  ___  _ __  ___| |    __ _ _ __   __| | __ ) / _ \_   _|
 | | | | '__/ _` |/ _` |/ _ \| '_ \|_  / |   / _` | '_ \ / _` |  _ \| | | || |  
 | |_| | | | (_| | (_| | (_) | | | |/ /| |__| (_| | | | | (_| | |_) | |_| || |  
 |____/|_|  \__,_|\__, |\___/|_| |_/___|_____\__,_|_| |_|\__,_|____/ \___/ |_|  
                  |___/                                                         ")
        Console.WriteLine()
        Console.WriteLine("Github: https://github.com/glad-tidings/DragonzLandBot-New")
        Console.WriteLine()
mainMenu:
        Console.Write("Select an option:
1. Run bot
2. Create session
>> ")
        Dim opt As String = Console.ReadLine()

        Dim queries As DragonzLandQuery()
        Dim jsonConfig As String = ""
        Dim jsonProxy As String = ""
        Try
            jsonConfig = File.ReadAllText("data.txt")
        Catch ex As Exception
            Console.WriteLine("file 'data.txt' not found")
            GoTo Get_Error
        End Try
        Try
            jsonProxy = File.ReadAllText("proxy.txt")
        Catch ex As Exception
            Console.WriteLine("file 'proxy.txt' not found")
            GoTo Get_Error
        End Try
        Try
            queries = JsonSerializer.Deserialize(Of DragonzLandQuery())(jsonConfig)
            proxies = JsonSerializer.Deserialize(Of Proxy())(jsonConfig)
        Catch ex As Exception
            Console.WriteLine("configuration is wrong")
            GoTo Get_Error
        End Try

        If Not String.IsNullOrEmpty(opt) Then
            Select Case opt
                Case "1"
                    Dim DragonzLand As New Thread(
                        Sub()
                            For Each Query In queries.Where(Function(x) x.Active)
                                Dim BotThread As New Thread(Sub() DragonzLandThread(Query))
                                BotThread.Start()

                                Thread.Sleep(120000)
                            Next
                        End Sub)
                    DragonzLand.Start()
                Case "2"
                    For Each Query In queries
                        If Not File.Exists($"sessions\{Query.Name}.session") Then
                            Console.WriteLine()
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})")
                            Dim vw As TelegramMiniApp.WebView = New TelegramMiniApp.WebView(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "")
                            If vw.Save_Session().Result Then
                                Console.WriteLine("Session created")
                            Else
                                Console.WriteLine("Create session failed")
                            End If
                        End If
                    Next

                    Environment.Exit(0)
                Case Else
                    GoTo mainMenu
            End Select
        Else
            GoTo mainMenu
        End If

Get_Error:
        Console.ReadLine()
    End Sub

    Public Async Sub DragonzLandThread(Query As DragonzLandQuery)
        While True
            Dim RND As New Random()
            Try
                Dim Bot As New DragonzLandBot(Query, proxies)
                If Not Bot.HasError Then
                    Log.Show("DragonzLand", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White)
                    Log.Show("DragonzLand", Query.Name, $"login successfully.", ConsoleColor.Green)
                    Dim Sync = Await Bot.DragonzLandUserDetail()
                    If Sync IsNot Nothing Then
                        Log.Show("DragonzLand", Query.Name, $"synced successfully. B<{Sync.Coins}> D<{Sync.Diamonds}> Po<{Sync.Power}> L<{Sync.Level}> E<{Sync.Energy}> Pr<{Sync.PassiveCoins.CoinsPerHour}>", ConsoleColor.Blue)
                        If String.IsNullOrEmpty(Sync.GuildId) Then
                            Dim joinGuild = Await Bot.DragonzLandGuildJoin("673c0f802f00542dea1ccaad")
                            If joinGuild Then
                                Log.Show("DragonzLand", Query.Name, $"join guild successfully", ConsoleColor.Green)
                            Else
                                Log.Show("DragonzLand", Query.Name, $"join guild failed", ConsoleColor.Red)
                            End If
                        Else
                            If Sync.GuildId <> "673c0f802f00542dea1ccaad" Then
                                Dim leaveGuild = Await Bot.DragonzLandGuildLeave()
                                If leaveGuild Then
                                    Thread.Sleep(2000)
                                    Dim joinGuild = Await Bot.DragonzLandGuildJoin("673c0f802f00542dea1ccaad")
                                    If joinGuild Then
                                        Log.Show("DragonzLand", Query.Name, $"join guild successfully", ConsoleColor.Green)
                                    Else
                                        Log.Show("DragonzLand", Query.Name, $"join guild failed", ConsoleColor.Red)
                                    End If
                                End If
                            End If
                        End If

                        Thread.Sleep(3000)

                        If Query.Task Then
                            Dim taskList = Await Bot.DragonzLandTasks()
                            If taskList IsNot Nothing Then
                                For Each task In taskList.Where(Function(x) x.Active = True And x.Recurrence = "daily")
                                    If task.LevelRecord IsNot Nothing Then
                                        If task.LevelRecord.ActiveAt.HasValue Then
                                            If task.LevelRecord.ActiveAt.Value.ToLocalTime > Date.Now Then Continue For
                                            If task.LevelRecord.Attempts = task.AttemptsLimit Then Continue For
                                        Else
                                            If task.LevelRecord.AttemptedAt.HasValue Then
                                                If task.LevelRecord.AttemptedAt.Value.ToLocalTime.AddDays(1) > Date.Now Then Continue For
                                            End If
                                        End If
                                    End If

                                    Dim claimTask = Await Bot.DragonzLandTasksVerify(task.Id)
                                    If claimTask Then
                                        Log.Show("DragonzLand", Query.Name, $"task '{task.Title}' completed", ConsoleColor.Green)
                                    Else
                                        Log.Show("DragonzLand", Query.Name, $"task '{task.Title}' failed", ConsoleColor.Red)
                                    End If

                                    Dim eachtaskRND As Integer = RND.Next(Query.TaskSleep(0), Query.TaskSleep(1))
                                    Thread.Sleep(eachtaskRND * 1000)
                                Next
                            End If
                        End If

                        If Query.Feed Then
                            Dim feedCount As Integer = Sync.Energy / Sync.FeedCoins
                            Dim feed = Await Bot.DragonzLandFeed(feedCount)
                            If feed Then
                                Log.Show("DragonzLand", Query.Name, $"'{feedCount}' taps completed", ConsoleColor.Green)
                            Else
                                Log.Show("DragonzLand", Query.Name, $"tap failed", ConsoleColor.Red)
                            End If

                            Thread.Sleep(3000)
                        End If

                        If Query.Boost Then
                            Dim syncBoost = Sync.Boosts.Where(Function(x) x.BoostId = "daily-energy-recharge")
                            Dim boostDo As Boolean = True
                            If syncBoost.Count <> 0 Then
                                If syncBoost(0).ActiveAt.HasValue Then
                                    If syncBoost(0).ActiveAt.Value.ToLocalTime > Date.Now Then boostDo = False
                                End If
                                If syncBoost(0).Attempts >= 3 Then boostDo = False
                            End If

                            If boostDo Then
                                Dim busBoost = Await Bot.DragonzLandBuyBoost("daily-energy-recharge")
                                If busBoost Then
                                    Log.Show("DragonzLand", Query.Name, $"buy 'Full Energy' completed", ConsoleColor.Green)

                                    Thread.Sleep(3000)

                                    Dim feedCount As Integer = Sync.Energy / Sync.FeedCoins
                                    Dim feed = Await Bot.DragonzLandFeed(feedCount)
                                    If feed Then
                                        Log.Show("DragonzLand", Query.Name, $"'{feedCount}' taps completed", ConsoleColor.Green)
                                    Else
                                        Log.Show("DragonzLand", Query.Name, $"tap failed", ConsoleColor.Red)
                                    End If

                                    Thread.Sleep(3000)
                                Else
                                    Log.Show("DragonzLand", Query.Name, $"buy 'Full Energy' failed", ConsoleColor.Red)
                                End If
                            End If
                        End If

                        If Query.BuyCard Then
                            Dim cards = Await Bot.DragonzLandCards()
                            For Each card In cards.Where(Function(x) x.Active And Not x.Claimed And x.ActiveAt < Date.Now And x.Currency = "coin" And x.TaskType <> "invite-more" And x.Cost < (Sync.Coins / 100)).OrderBy(Function(x) x.Cost)
                                If card.TaskId <> "" Then
                                    Dim verify = Await Bot.DragonzLandVerifyCard(card.TaskId, card.Id)
                                    If verify Then
                                        Log.Show("DragonzLand", Query.Name, $"card '{card.Title}' verified", ConsoleColor.Green)

                                        Thread.Sleep(3000)

                                        Dim buy = Await Bot.DragonzLandBuyCard(card.Id)
                                        If buy Then
                                            Log.Show("DragonzLand", Query.Name, $"card '{card.Title}' bought", ConsoleColor.Green)
                                        Else
                                            Log.Show("DragonzLand", Query.Name, $"buy card '{card.Title}' failed", ConsoleColor.Red)
                                        End If
                                    Else
                                        Log.Show("DragonzLand", Query.Name, $"verify card '{card.Title}' failed", ConsoleColor.Red)
                                    End If
                                Else
                                    Dim buy = Await Bot.DragonzLandBuyCard(card.Id)
                                    If buy Then
                                        Log.Show("DragonzLand", Query.Name, $"card '{card.Title}' bought", ConsoleColor.Green)
                                    Else
                                        Log.Show("DragonzLand", Query.Name, $"buy card '{card.Title}' failed", ConsoleColor.Red)
                                    End If
                                End If

                                Dim eachcardRND As Integer = RND.Next(Query.BuyCardSleep(0), Query.BuyCardSleep(1))
                                Thread.Sleep(eachcardRND * 1000)
                            Next
                        End If
                    Else
                        Log.Show("DragonzLand", Query.Name, $"synced failed", ConsoleColor.Red)
                    End If

                    Sync = Await Bot.DragonzLandUserDetail()
                    If Sync IsNot Nothing Then
                        Log.Show("DragonzLand", Query.Name, $"B<{Sync.Coins}> D<{Sync.Diamonds}> Po<{Sync.Power}> L<{Sync.Level}> E<{Sync.Energy}> Pr<{Sync.PassiveCoins.CoinsPerHour}>", ConsoleColor.Blue)
                    End If
                Else
                    Log.Show("DragonzLand", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red)
                End If
            Catch ex As Exception
                Log.Show("DragonzLand", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red)
            End Try

            Dim syncRND As Integer = 0
            If Date.Now.Hour < 8 Then
                syncRND = RND.Next(Query.NightSleep(0), Query.NightSleep(1))
            Else
                syncRND = RND.Next(Query.DaySleep(0), Query.DaySleep(1))
            End If
            Log.Show("DragonzLand", Query.Name, $"sync sleep '{Int(syncRND / 3600)}h {Int((syncRND Mod 3600) / 60)}m {syncRND Mod 60}s'", ConsoleColor.Yellow)
            Thread.Sleep(syncRND * 1000)
        End While
    End Sub
End Module
