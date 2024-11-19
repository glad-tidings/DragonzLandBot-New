Imports System.Text.Json.Serialization

Public Class DragonzLandQuery
    Public Property Index As Long
    Public Property Name As String
    Public Property API_ID As String
    Public Property API_HASH As String
    Public Property Phone As String
    Public Property Auth As String
    Public Property Active As Boolean
    Public Property Tap As Boolean
    Public Property Task As Boolean
    Public Property TaskSleep As Integer()
    Public Property Feed As Boolean
    Public Property Boost As Boolean
    Public Property BuyCard As Boolean
    Public Property BuyCardSleep As Integer()
    Public Property DaySleep As Integer()
    Public Property NightSleep As Integer()
End Class

Public Class DragonzLandAuthRequest
    <JsonPropertyName("initData")>
    Public Property InitData As String
End Class

Public Class DragonzLandAuthResponse
    <JsonPropertyName("accessToken")>
    Public Property AccessToken As String
    <JsonPropertyName("refreshToken")>
    Public Property RefreshToken As String
End Class

Public Class DragonzLandMeResponse
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("guildId")>
    Public Property GuildId As String
    <JsonPropertyName("telegramUserId")>
    Public Property TelegramUserId As Long
    <JsonPropertyName("username")>
    Public Property Username As String
    <JsonPropertyName("firstName")>
    Public Property FirstName As String
    <JsonPropertyName("lastName")>
    Public Property LastName As String
    <JsonPropertyName("diamonds")>
    Public Property Diamonds As Integer
    <JsonPropertyName("coins")>
    Public Property Coins As Integer
    <JsonPropertyName("power")>
    Public Property Power As Integer
    <JsonPropertyName("feedCoins")>
    Public Property FeedCoins As Integer
    <JsonPropertyName("passiveCoins")>
    Public Property PassiveCoins As DragonzLandMePassive
    <JsonPropertyName("level")>
    Public Property Level As Integer
    <JsonPropertyName("energy")>
    Public Property Energy As Integer
    <JsonPropertyName("energyLimit")>
    Public Property EnergyLimit As Integer
    '<JsonPropertyName("tasks")>
    'Public Property Tasks As List(Of DragonzLandMeTask)
    <JsonPropertyName("boosts")>
    Public Property Boosts As List(Of DragonzLandMeBoost)
    <JsonPropertyName("cards")>
    Public Property Cards As List(Of DragonzLandMeCard)
End Class

Public Class DragonzLandMePassive
    <JsonPropertyName("coinsPerHour")>
    Public Property CoinsPerHour As Integer
End Class

'Public Class DragonzLandMeTask
'    <JsonPropertyName("taskId")>
'    Public Property TaskId As String
'    <JsonPropertyName("level")>
'    Public Property Level As Integer
'    <JsonPropertyName("attempts")>
'    Public Property Attempts As Integer
'    <JsonPropertyName("attemptedAt")>
'    Public Property AttemptedAt As DateTime?
'    <JsonPropertyName("activeAt")>
'    Public Property ActiveAt As DateTime?
'End Class

Public Class DragonzLandMeBoost
    <JsonPropertyName("boostId")>
    Public Property BoostId As String
    <JsonPropertyName("level")>
    Public Property Level As Integer
    <JsonPropertyName("attempts")>
    Public Property Attempts As Integer
    <JsonPropertyName("attemptedAt")>
    Public Property AttemptedAt As DateTime?
    <JsonPropertyName("activeAt")>
    Public Property ActiveAt As DateTime?
End Class

Public Class DragonzLandMeCard
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("cardId")>
    Public Property CardId As String
    <JsonPropertyName("level")>
    Public Property Level As Integer
    <JsonPropertyName("attemptedAt")>
    Public Property AttemptedAt As DateTime?
    <JsonPropertyName("activeAt")>
    Public Property ActiveAt As DateTime?
End Class

Public Class DragonzLandTaskResponse
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("type")>
    Public Property Type As String
    <JsonPropertyName("recurrence")>
    Public Property Recurrence As String
    <JsonPropertyName("attemptsLimit")>
    Public Property AttemptsLimit As Integer
    <JsonPropertyName("active")>
    Public Property Active As Boolean
    <JsonPropertyName("featured")>
    Public Property Featured As Boolean
    <JsonPropertyName("pinned")>
    Public Property Pinned As Boolean
    <JsonPropertyName("listed")>
    Public Property Listed As Boolean
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("levelRecord")>
    Public Property LevelRecord As DragonzLandTaskLevelRecord
End Class

Public Class DragonzLandTaskLevelRecord
    <JsonPropertyName("taskId")>
    Public Property TaskId As String
    <JsonPropertyName("level")>
    Public Property Level As Integer
    <JsonPropertyName("attempts")>
    Public Property Attempts As Integer
    <JsonPropertyName("attemptedAt")>
    Public Property AttemptedAt As DateTime?
    <JsonPropertyName("activeAt")>
    Public Property ActiveAt As DateTime?
End Class

Public Class DragonzLandTaskDoneRequest
    <JsonPropertyName("taskId")>
    Public Property TaskId As String
End Class

Public Class DragonzLandFeedRequest
    <JsonPropertyName("feedCount")>
    Public Property FeedCount As Integer
End Class

Public Class DragonzLandBoostResponse
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("type")>
    Public Property Type As String
    <JsonPropertyName("active")>
    Public Property Active As Boolean
    <JsonPropertyName("comingSoon")>
    Public Property ComingSoon As Boolean
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("attemptsLimit")>
    Public Property AttemptsLimit As Integer
    <JsonPropertyName("recurrence")>
    Public Property Recurrence As String
End Class

Public Class DragonzLandBoostBuyRequest
    <JsonPropertyName("boostId")>
    Public Property BoostId As String
End Class

Public Class DragonzLandCards
    Public Property Id As String
    Public Property Title As String
    Public Property CategoryId As String
    Public Property Active As Boolean
    Public Property Featured As Boolean
    Public Property Pinned As Boolean
    Public Property Level As Integer
    Public Property Claimed As Boolean
    Public Property Currency As String
    Public Property Cost As Integer
    Public Property PassiveCoins As Integer
    Public Property ActiveAt As DateTime
    Public Property TaskId As String
    Public Property TaskType As String
    Public Property TaskProgress As String
End Class

Public Class DragonzLandCardResponse
    <JsonPropertyName("items")>
    Public Property Items As List(Of DragonzLandCardItem)
    <JsonPropertyName("page")>
    Public Property Page As Integer
    <JsonPropertyName("totalPages")>
    Public Property TotalPages As Integer
    <JsonPropertyName("totalElements")>
    Public Property TotalElements As Integer
End Class

Public Class DragonzLandCardItem
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("categoryId")>
    Public Property CategoryId As String
    <JsonPropertyName("active")>
    Public Property Active As Boolean
    <JsonPropertyName("featured")>
    Public Property Featured As Boolean
    <JsonPropertyName("pinned")>
    Public Property Pinned As Boolean
    <JsonPropertyName("levelRecord")>
    Public Property LevelRecord As DragonzLandCardItemLevelRecord
    <JsonPropertyName("levels")>
    Public Property Levels As List(Of DragonzLandCardItemLevel)
End Class

Public Class DragonzLandCardItemLevelRecord
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("level")>
    Public Property Level As Integer
    <JsonPropertyName("claimed")>
    Public Property Claimed As Boolean
    <JsonPropertyName("attemptedAt")>
    Public Property AttemptedAt As DateTime
    <JsonPropertyName("activeAt")>
    Public Property ActiveAt As DateTime
End Class

Public Class DragonzLandCardItemLevel
    <JsonPropertyName("currency")>
    Public Property Currency As String
    <JsonPropertyName("cost")>
    Public Property Cost As Integer
    <JsonPropertyName("coins")>
    Public Property Coins As Integer
    <JsonPropertyName("passiveCoins")>
    Public Property PassiveCoins As Integer
    <JsonPropertyName("tasks")>
    Public Property Tasks As List(Of DragonzLandCardItemLevelTask)
End Class

Public Class DragonzLandCardItemLevelTask
    <JsonPropertyName("id")>
    Public Property Id As String
    <JsonPropertyName("type")>
    Public Property [Type] As String
    <JsonPropertyName("recurrence")>
    Public Property Recurrence As String
    <JsonPropertyName("levelRecord")>
    Public Property LevelRecord As DragonzLandCardItemLevelTaskLevelRecord
End Class

Public Class DragonzLandCardItemLevelTaskLevelRecord
    <JsonPropertyName("progress")>
    Public Property Progress As String
End Class

Public Class DragonzLandCardVerifyRequest
    <JsonPropertyName("taskId")>
    Public Property TaskId As String
    <JsonPropertyName("cardId")>
    Public Property CardId As String
End Class

Public Class DragonzLandCardBuyRequest
    <JsonPropertyName("cardId")>
    Public Property CardId As String
End Class

Public Class Proxy
    Public Property Index As Integer
    Public Property Proxy As String
End Class

Public Class httpbin
    <JsonPropertyName("origin")>
    Public Property Origin As String
End Class

Public Class DragonzLandGuildRequest
    <JsonPropertyName("guildId")>
    Public Property GuildId As String
End Class