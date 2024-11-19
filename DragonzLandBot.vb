Imports System.Net
Imports System.Net.Http
Imports System.Text
Imports System.Text.Json
Imports System.Threading

Public Class DragonzLandBot

    Public ReadOnly PubQuery As DragonzLandQuery
    Private ReadOnly PubProxy As Proxy()
    Private ReadOnly AccessToken As String
    Public ReadOnly HasError As Boolean
    Public ReadOnly ErrorMessage As String
    Public ReadOnly IPAddress As String

    Public Sub New(Query As DragonzLandQuery, Proxy As Proxy())
        PubQuery = Query
        PubProxy = Proxy
        IPAddress = GetIP().Result
        PubQuery.Auth = getSession().Result
        Dim GetToken = DragonzLandGetToken().Result
        If GetToken IsNot Nothing Then
            AccessToken = GetToken.AccessToken
            HasError = False
            ErrorMessage = ""
        Else
            HasError = True
            ErrorMessage = "get token failed"
        End If
    End Sub

    Private Async Function GetIP() As Task(Of String)
        Dim client As HttpClient
        Dim FProxy = PubProxy.Where(Function(x) x.Index = PubQuery.Index)
        If FProxy.Count <> 0 Then
            If FProxy(0).Proxy <> "" Then
                Dim handler = New HttpClientHandler With {.Proxy = New WebProxy With {.Address = New Uri(FProxy(0).Proxy)}}
                client = New HttpClient(handler) With {.Timeout = New TimeSpan(0, 0, 30)}
            Else
                client = New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
            End If
        Else
            client = New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
        End If
        Dim httpResponse As HttpResponseMessage = Nothing
        Try
            httpResponse = Await client.GetAsync($"https://httpbin.org/ip")
        Catch ex As Exception
        End Try
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of httpbin)(responseStream)
                Return responseJson.Origin
            Else
                Return ""
            End If
        Else
            Return ""
        End If
    End Function

    Private Async Function getSession() As Task(Of String)
        Dim vw As TelegramMiniApp.WebView = New TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "dragonz_land_bot", "https://bot.dragonz.land/")
        Dim url As String = Await vw.Get_URL()
        If url <> "" Then
            Return url.Split(New String() {"tgWebAppData="}, StringSplitOptions.None)(1).Split(New String() {"&tgWebAppVersion"}, StringSplitOptions.None)(0)
        Else
            Return ""
        End If
    End Function

    Private Async Function DragonzLandGetToken() As Task(Of DragonzLandAuthResponse)
        Dim DLAPI As New DragonzLandApi(0, PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandAuthRequest With {.InitData = PubQuery.Auth}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/auth/telegram", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of DragonzLandAuthResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function DragonzLandUserDetail() As Task(Of DragonzLandMeResponse)
        Dim rDLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim rewardResponse = Await rDLAPI.DLAPIGet("https://bot.dragonz.land/api/tasks/welcome-reward")
        Thread.Sleep(3000)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim httpResponse = Await DLAPI.DLAPIGet("https://bot.dragonz.land/api/me")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of DragonzLandMeResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function DragonzLandTasks() As Task(Of List(Of DragonzLandTaskResponse))
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim httpResponse = Await DLAPI.DLAPIGet("https://bot.dragonz.land/api/tasks")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of List(Of DragonzLandTaskResponse))(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function DragonzLandTasksVerify(taskId As String) As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandTaskDoneRequest With {.TaskId = taskId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/tasks/verify", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

    Public Async Function DragonzLandFeed(feedCount As Integer) As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandFeedRequest With {.FeedCount = feedCount}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/feed", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

    Public Async Function DragonzLandBuyBoost(boostId As String) As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandBoostBuyRequest With {.BoostId = boostId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/boosts/buy", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

    Public Async Function DragonzLandCards() As Task(Of List(Of DragonzLandCards))
        Dim DLC As New List(Of DragonzLandCards)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim httpResponse = Await DLAPI.DLAPIGet("https://bot.dragonz.land/api/cards")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of DragonzLandCardResponse)(responseStream)
                If responseJson.Items IsNot Nothing Then
                    For Each item In responseJson.Items
                        Dim EDLC As New DragonzLandCards With {
                            .Active = item.Active,
                            .CategoryId = item.CategoryId,
                            .Featured = item.Featured,
                            .Id = item.Id,
                            .Pinned = item.Pinned,
                            .Title = item.Title
                        }
                        If item.LevelRecord IsNot Nothing Then
                            EDLC.Level = item.LevelRecord.Level
                            EDLC.Claimed = item.LevelRecord.Claimed
                            EDLC.ActiveAt = item.LevelRecord.ActiveAt.ToLocalTime
                        Else
                            EDLC.Level = 0
                            EDLC.Claimed = False
                            EDLC.ActiveAt = Date.Now
                        End If
                        If item.Levels IsNot Nothing Then
                            If item.Levels.Count > (EDLC.Level + 1) Then
                                EDLC.Cost = item.Levels(EDLC.Level + 1).Cost
                                EDLC.Currency = item.Levels(EDLC.Level + 1).Currency
                                EDLC.PassiveCoins = item.Levels(EDLC.Level + 1).PassiveCoins
                                If item.Levels(EDLC.Level + 1).Tasks.Count = 1 Then
                                    EDLC.TaskId = item.Levels(EDLC.Level + 1).Tasks(0).Id
                                    EDLC.TaskType = item.Levels(EDLC.Level + 1).Tasks(0).Type
                                    If item.Levels(EDLC.Level + 1).Tasks(0).LevelRecord IsNot Nothing Then
                                        EDLC.TaskProgress = item.Levels(EDLC.Level + 1).Tasks(0).LevelRecord.Progress
                                    Else
                                        EDLC.TaskProgress = ""
                                    End If
                                Else
                                    EDLC.TaskId = ""
                                    EDLC.TaskType = ""
                                    EDLC.TaskProgress = ""
                                End If
                            Else
                                EDLC.Cost = 0
                                EDLC.Currency = "coin"
                                EDLC.PassiveCoins = 0
                                EDLC.TaskId = ""
                                EDLC.TaskProgress = ""
                            End If
                        Else
                            EDLC.Cost = 0
                            EDLC.Currency = "coin"
                            EDLC.PassiveCoins = 0
                            EDLC.TaskId = ""
                            EDLC.TaskProgress = ""
                        End If
                        DLC.Add(EDLC)
                    Next

                    For I As Integer = 1 To responseJson.TotalPages - 1
                        Thread.Sleep(2000)
                        httpResponse = Await DLAPI.DLAPIGet($"https://bot.dragonz.land/api/cards?page={I}")
                        If httpResponse IsNot Nothing Then
                            If httpResponse.IsSuccessStatusCode Then
                                responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                                responseJson = Await JsonSerializer.DeserializeAsync(Of DragonzLandCardResponse)(responseStream)
                                If responseJson.Items IsNot Nothing Then
                                    For Each item In responseJson.Items
                                        Dim EDLC As New DragonzLandCards With {
                                            .Active = item.Active,
                                            .CategoryId = item.CategoryId,
                                            .Featured = item.Featured,
                                            .Id = item.Id,
                                            .Pinned = item.Pinned,
                                            .Title = item.Title
                                        }
                                        If item.LevelRecord IsNot Nothing Then
                                            EDLC.Level = item.LevelRecord.Level
                                            EDLC.Claimed = item.LevelRecord.Claimed
                                            EDLC.ActiveAt = item.LevelRecord.ActiveAt.ToLocalTime
                                        Else
                                            EDLC.Level = 0
                                            EDLC.Claimed = False
                                            EDLC.ActiveAt = Date.Now
                                        End If
                                        If item.Levels IsNot Nothing Then
                                            If item.Levels.Count > (EDLC.Level + 1) Then
                                                EDLC.Cost = item.Levels(EDLC.Level + 1).Cost
                                                EDLC.Currency = item.Levels(EDLC.Level + 1).Currency
                                                EDLC.PassiveCoins = item.Levels(EDLC.Level + 1).PassiveCoins
                                                If item.Levels(EDLC.Level + 1).Tasks.Count = 1 Then
                                                    EDLC.TaskId = item.Levels(EDLC.Level + 1).Tasks(0).Id
                                                    EDLC.TaskType = item.Levels(EDLC.Level + 1).Tasks(0).Type
                                                    If item.Levels(EDLC.Level + 1).Tasks(0).LevelRecord IsNot Nothing Then
                                                        EDLC.TaskProgress = item.Levels(EDLC.Level + 1).Tasks(0).LevelRecord.Progress
                                                    Else
                                                        EDLC.TaskProgress = ""
                                                    End If
                                                Else
                                                    EDLC.TaskId = ""
                                                    EDLC.TaskType = ""
                                                    EDLC.TaskProgress = ""
                                                End If
                                            Else
                                                EDLC.Cost = 0
                                                EDLC.Currency = "coin"
                                                EDLC.PassiveCoins = 0
                                                EDLC.TaskId = ""
                                                EDLC.TaskProgress = ""
                                            End If
                                        Else
                                            EDLC.Cost = 0
                                            EDLC.Currency = "coin"
                                            EDLC.PassiveCoins = 0
                                            EDLC.TaskId = ""
                                            EDLC.TaskProgress = ""
                                        End If
                                        DLC.Add(EDLC)
                                    Next
                                End If
                            End If
                        End If
                    Next

                    Return DLC
                Else
                    Return Nothing
                End If
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function DragonzLandVerifyCard(taskId As String, cardId As String) As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandCardVerifyRequest With {.TaskId = taskId, .CardId = cardId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/tasks/verify", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

    Public Async Function DragonzLandBuyCard(cardId As String) As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandCardBuyRequest With {.CardId = cardId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/cards/buy", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

    Public Async Function DragonzLandGuildJoin(guildId As String) As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim request As New DragonzLandGuildRequest With {.GuildId = guildId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/guild/join", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

    Public Async Function DragonzLandGuildLeave() As Task(Of Boolean)
        Dim DLAPI As New DragonzLandApi(1, AccessToken, PubQuery.Index, PubProxy)
        Dim httpResponse = Await DLAPI.DLAPIPost("https://bot.dragonz.land/api/me/guild/leave", Nothing)
        If httpResponse IsNot Nothing Then
            Return httpResponse.IsSuccessStatusCode
        Else
            Return False
        End If
    End Function

End Class
