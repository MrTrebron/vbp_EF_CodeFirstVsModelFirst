Imports System.Data.Entity
Imports CodeFirst_Context
Imports CodeFirst_Model

Module Module1
    Private _loggingOn As Boolean = False
    Sub Main()


        Console.Write("EF Logging einschalten? J|N: ")
        Dim selectionKey = Console.ReadKey
        Select Case selectionKey.Key
            Case ConsoleKey.J
                _loggingOn = True
        End Select
        WriteToConsole(String.Empty)

        Console.WriteLine("Initialisiere Datenbankschema und prüfe ob DB existiert.")
        Using db As New BloggingContext(_loggingOn)

            WriteToConsole(String.Empty)
            WriteToConsole($"DB Connectionstring: {db.Database.Connection.ConnectionString}", ConsoleColor.Gray)
            WriteToConsole(String.Empty)
            db.Database.Delete
            If Not db.Database.Exists Then
                WriteToConsole(String.Empty) : WriteToConsole("DB existiert nicht, wird automatisch angelegt...", ConsoleColor.Red, True)
                db.Database.Create()
                WriteToConsole("DB wurde angelegt!", ConsoleColor.Red, True) : WriteToConsole(String.Empty)
            Else
                WriteToConsole("DB existiert bereits!", ConsoleColor.Green, True) : WriteToConsole(String.Empty)
            End If
            WriteToConsole("Migriere Schema und führe Seeding aus ...")
            db.Database.Initialize(True)

            If Not db.Blogs.Any Then
                WriteToConsole("Fehler, es sollte Blogs geben, aber in der DB sind keine vorhanden, bitte prüfen", ConsoleColor.Red)
                Console.Read()
            End If

        End Using
        WriteToConsole("==================================================")
        WriteToConsole($"DB initialisiert und erster Abruf (ANY) abgesetzt.")
        WriteToConsole("==================================================")

        StartMultipleChoice()



        Console.Read()
    End Sub


    Private Sub StartMultipleChoice()
        WriteToConsole(String.Empty) : WriteToConsole(String.Empty)
        WriteToConsole("Was willst du als nächstes machen?")
        WriteToConsole("1.) Posts von EDV Blog abrufen")
        WriteToConsole("2.) Post schreiben")
        WriteToConsole("3.) User abrufen")
        WriteToConsole("4.) Neuen User anlegen")
        WriteToConsole("5.) User editieren")

        WriteToConsole(String.Empty)
        Console.Write("Deine Wahl:")
        Dim selectionKey = Console.ReadKey
        Select Case selectionKey.Key
            Case ConsoleKey.NumPad1, ConsoleKey.D1
                MultipleChoice_GetPosts()
            Case ConsoleKey.NumPad2, ConsoleKey.D2
                MultipleChoice_WritePost()
            Case ConsoleKey.NumPad3, ConsoleKey.D3
                MultipleChoice_GetUser()
            Case ConsoleKey.NumPad4, ConsoleKey.D4
                MultipleChoice_AddUser()
            Case ConsoleKey.NumPad5, ConsoleKey.D5
                MultipleChoice_EditUser()
            Case Else
                WriteToConsole("Leider wurde deine Eingabe nicht verstanden, versuche es bitte nochmals...", ConsoleColor.Red, True)

        End Select
        StartMultipleChoice()
    End Sub



#Region "MultipleChoice Subs"

    Private Sub MultipleChoice_GetPosts()
        WriteToConsole(String.Empty)
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        Using db As New BloggingContext(_loggingOn)

            WriteToConsole("* GetPost: Erstelle Post-Queryable...", ConsoleColor.Yellow)
            Dim postQuery = db.Posts.Include(Function(b) b.Blog).Include(Function(p) p.MarkedUsers).Include(Function(p) p.OpenedBy) 'Eager Loading sonst werden viel zu viele Anfragen an die DB gesendet anstatt Joins zu machen
            postQuery = postQuery.Where(Function(p) Not p.DeletedFlag) 'Ohne als gelöscht markierte
            postQuery = postQuery.OrderBy(Function(o) o.CreationTimestamp.Value)
            WriteToConsole("* GetPost: Queryable generiert!", ConsoleColor.Yellow)

            Dim tempPosts = postQuery.AsNoTracking.Select(Function(p) New With {p.Title, p.Content, p.MarkedUsers, p.OpenedBy}).ToList

            WriteToConsole($"* GetPost: Posts mit ToList abgerufen... {tempPosts.Count} Posts gefunden!")

            WriteToConsole(String.Empty, ConsoleColor.Yellow)
            WriteToConsole("Alle Posts mit deren Eigenschaften und Verknüpfungen...")
            For Each post In tempPosts
                WriteToConsole("*** Post Anfang ***")
                WriteToConsole(post.ToString, ConsoleColor.Green)
                WriteToConsole(String.Empty)
                WriteToConsole("* GetPost: Folgende User wurden Markiert:")
                For Each marked In post.MarkedUsers
                    WriteToConsole(marked.ToString, ConsoleColor.Green)
                Next
                WriteToConsole("*** Post ENDE ***") : WriteToConsole(String.Empty)
            Next
        End Using
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        WriteToConsole(String.Empty)
    End Sub


    Private Sub MultipleChoice_WritePost()
        WriteToConsole(String.Empty)
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        Using db As New BloggingContext(_loggingOn)
            WriteToConsole("* WritePost: Erstelle neues Post-Objekt im ersten verfügbaren Blog...", ConsoleColor.Yellow)

            Dim p As New Post
            p.Blog = db.Blogs.First

            Console.Write("Gebe den Titel an:")
            p.Title = Console.ReadLine
            Console.Write("Gebe den Content an:")
            p.Content = Console.ReadLine

            Console.WriteLine(String.Empty)

            Do
                Console.WriteLine("Folgende User gibt es, gebe die ID des Users an welcher als verfasser gelten soll.")
                WriteToConsole(String.Join(Environment.NewLine, db.Users.ToList))

                Dim typedUserId As Integer : Dim wasIntegerTyped As Boolean
                Do
                    Console.Write("Gebe die UserID des Verfassers an:")
                    wasIntegerTyped = Integer.TryParse(Console.ReadLine, typedUserId)
                    If Not wasIntegerTyped Then WriteToConsole("Du hast keine Ziffer eingegeben. Probieren wir es nochmal...", ConsoleColor.Red, True)
                Loop Until wasIntegerTyped
                p.OpenedBy = db.Users.Where(Function(u) u.Id = typedUserId).FirstOrDefault
                If p.OpenedBy Is Nothing Then WriteToConsole("Unbekannte UserID. Probieren wir es nochmal...", ConsoleColor.Red, True)
            Loop Until p.OpenedBy IsNot Nothing


            Console.WriteLine("Gebe Kommatagetrennt die ID der User welche in dem Post markiert werden sollen ein. Hier alle User:")
            WriteToConsole(String.Join(Environment.NewLine, db.Users.ToList))

            Dim allUsersFound As Boolean = False
            Dim foundedUsers As New List(Of User)
            Do
                Console.Write("Gebe die UserID`s der User ein welche markiert werden sollen:")
                Dim usersString As String = Console.ReadLine
                Try
                    Dim usersList As List(Of String) = usersString.Split(CType(";", Char)).ToList
                    usersList.ForEach(Sub(u) foundedUsers.Add(db.Users.Find(CInt(u))))


                    allUsersFound = True
                    p.MarkedUsers = New List(Of User)
                    foundedUsers.ForEach(Sub(u) p.MarkedUsers.Add(u))

                Catch ex As Exception
                    WriteToConsole(ex.Message, ConsoleColor.Red, True)
                End Try

            Loop Until allUsersFound

            db.Posts.Add(p)
            WriteToConsole("Alle nötigen Angaben vorhanden, speichere...")
            Dim contextChangesCount As Integer = db.SaveChanges
            If contextChangesCount > 0 Then
                WriteToConsole(
                    $"Es wurden {contextChangesCount.ToString _
                                  } Datensätze gespeichert. Toll, du hast einen Post angelegt!!!")
            Else
                WriteToConsole("Anscheinend ist ein Fehler passiert da keine Datensätze in der DB gespeichert wurden. Sorry", ConsoleColor.Red, True)
            End If


        End Using
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        WriteToConsole(String.Empty)
    End Sub


    Private Sub MultipleChoice_GetUser()
        WriteToConsole(String.Empty)
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        Using db As New BloggingContext(_loggingOn)

            WriteToConsole("* GetUser: Erstelle User-Queryable...", ConsoleColor.Yellow)
            Dim userQuery = db.Users.Include(Function(u) u.MarkedInPosts).Include(Function(p) p.OpenedPosts) 'Eager Loading sonst werden viel zu viele Anfragen an die DB gesendet anstatt Joins zu machen
            userQuery = userQuery.Where(Function(u) Not u.DeletedFlag) 'Ohne als gelöscht markierte
            userQuery = userQuery.OrderBy(Function(o) o.UserName)
            WriteToConsole("* GetUser: Queryable generiert!", ConsoleColor.Yellow)

            Dim tempUsers = userQuery.AsNoTracking.Select(Function(u) New With {u.UserName, u.UserMailaddress, u.Id, u.IsActive, u.LastUpdateBy, u.LastUpdateTimestamp, u.OpenedPosts, u.MarkedInPosts, u.Birthday}).ToList
            WriteToConsole($"* GetUser: User mit ToList abgerufen... {tempUsers.Count} User gefunden!")

            WriteToConsole(String.Empty, ConsoleColor.Yellow)
            WriteToConsole("Alle Users mit deren Eigenschaften und Verknüpfungen...")
            For Each u In userQuery.ToList
                WriteToConsole("*** User Anfang ***")
                WriteToConsole(u.ToString, ConsoleColor.Green)
                WriteToConsole(String.Empty)
                WriteToConsole("* GetUser: In folgenden Posts wurde der User markiert:")
                For Each marked In u.MarkedInPosts
                    WriteToConsole(marked.ToString, ConsoleColor.Green)
                Next
                WriteToConsole("* GetUser: Folgende Posts hat der User eröffnet:")
                For Each opened In u.OpenedPosts
                    WriteToConsole(opened.ToString, ConsoleColor.Green)
                Next
                WriteToConsole("*** User ENDE ***") : WriteToConsole(String.Empty)
            Next
        End Using
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        WriteToConsole(String.Empty)

    End Sub


    Private Sub MultipleChoice_AddUser()
        WriteToConsole(String.Empty)
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        Using db As New BloggingContext(_loggingOn)
            WriteToConsole("* AddUser: Erstelle neues User-Objekt im ersten verfügbaren Blog...", ConsoleColor.Yellow)

            Dim u As New User

            Console.Write("Gebe den Username an:")
            u.UserName = Console.ReadLine
            Console.Write("Gebe die Mailadresse an:")
            u.UserMailaddress = Console.ReadLine
            Console.Write("Gebe das Geburtsdatum an:")

            Dim bd As Date
            Do
                Dim bdString As String = Console.ReadLine

                If Date.TryParse(bdString, bd) Then
                    u.Birthday = bd
                End If
            Loop Until bd > Date.MinValue

            Console.WriteLine(String.Empty)

            db.Users.Add(u)
            WriteToConsole("Alle nötigen Angaben vorhanden, speichere...")
            Dim contextChangesCount As Integer = db.SaveChanges
            If contextChangesCount > 0 Then
                WriteToConsole(
                    $"Es wurden {contextChangesCount.ToString _
                                  } Datensätze gespeichert. Toll, du hast einen User angelegt!!!")
            Else
                WriteToConsole("Anscheinend ist ein Fehler passiert da keine Datensätze in der DB gespeichert wurden. Sorry", ConsoleColor.Red, True)
            End If

        End Using
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        WriteToConsole(String.Empty)
    End Sub

    Private Sub MultipleChoice_EditUser()
        WriteToConsole(String.Empty)
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        Using db As New BloggingContext(_loggingOn)
            WriteToConsole("* EditUser: Editiere User-Objekt...", ConsoleColor.Yellow)
            Console.Write("Welchen User willst du editieren? Gebe die ID an:")

            Dim uId As Integer = 0
            Integer.TryParse(Console.ReadLine, uId)
            Dim u As User = db.Users.Find(uId)

            If u Is Nothing Then
                WriteToConsole("Leider konnte der gesuchte User nicht gefunden werden, bitte versuche es nochmals.")
                StartMultipleChoice()
            End If

            Console.Write("Aktueller Username: {0}. Gebe den neuen Username an:", u.UserName)
            u.UserName = Console.ReadLine
            Console.Write("Aktuelle Mailadresse: {0}. Gebe die neue Mailadresse an:", u.UserMailaddress)
            u.UserMailaddress = Console.ReadLine

            Dim bd As Date
            Do
                Console.Write("Aktuelles Geburtsdatum: {0}. Gebe das Geburtsdatum an:", u.Birthday.Value.ToShortDateString)
                Dim bdString As String = Console.ReadLine

                If Date.TryParse(bdString, bd) Then
                    u.Birthday = bd
                Else
                    WriteToConsole("Leider ist dies kein korrektes Datum, versuchen wir es nochmals...", ConsoleColor.Red, True)
                End If
            Loop Until bd > Date.MinValue

            Console.WriteLine(String.Empty)

            WriteToConsole("Alle nötigen Angaben vorhanden, speichere...")
            Dim contextChangesCount As Integer = db.SaveChanges
            If contextChangesCount > 0 Then
                WriteToConsole(
                    $"Es wurden {contextChangesCount.ToString _
                                  } Datensätze gespeichert. Toll, du hast einen User editiert!!!")
            Else
                WriteToConsole("Anscheinend ist ein Fehler passiert da keine Datensätze in der DB gespeichert wurden. Sorry", ConsoleColor.Red, True)
            End If

        End Using
        WriteToConsole("========================================================", ConsoleColor.Yellow)
        WriteToConsole(String.Empty)
    End Sub

#End Region



#Region "Console Helper"

    Private Sub WriteToConsole(text As String, Optional color As ConsoleColor = ConsoleColor.White, Optional center As Boolean = False)
        Dim oldColor As ConsoleColor = Console.ForegroundColor
        Console.ForegroundColor = color

        If text.Length > Console.WindowWidth AndAlso center Then
            Dim stringList = SplitLength(text, Console.WindowWidth)
            For Each item In stringList
                Console.SetCursorPosition(CType((Console.WindowWidth / 2 - item.Length / 2), Integer), Console.CursorTop)
                Console.WriteLine(item)
            Next
        Else
            If center Then Console.SetCursorPosition(CType((Console.WindowWidth / 2 - text.Length / 2), Integer), Console.CursorTop)
            Console.WriteLine(text)
        End If

        Console.ForegroundColor = oldColor
    End Sub


    Private Function SplitLength(ByVal value As String, ByVal num As Integer) As String()
        Dim ret(CInt(Math.Ceiling(value.Length / num)) - 1) As String
        For i As Integer = 0 To ret.Length - 1
            Dim start As Integer = i * num
            Dim ende As Integer = value.Length - start
            If ende > num Then
                ende = num
            End If
            ret(i) = value.Substring(start, ende)
        Next
        Return ret
    End Function
#End Region

End Module



