Imports System.Data.Entity.Migrations
Imports CodeFirst_Model

Namespace Migrations

    Friend NotInheritable Class Configuration
        Inherits DbMigrationsConfiguration(Of BloggingContext)

        Public Sub New()

            AutomaticMigrationsEnabled = True
            AutomaticMigrationDataLossAllowed = True
            
        End Sub

        Protected Overrides Sub Seed(context As BloggingContext)
            Console.WriteLine("*** Seeding ***")
            Console.Write("AddOrUpdate für User...")
            context.Users.AddOrUpdate(Function(u) u.UserMailaddress,
                                      New User("NoFear23m", "nofear23m@gmail.com", New Date(1983, 9, 12), True),
                                      New User("Testuser", "test@domain.com", New Date(1977, 11, 3), True),
                                                                            New User("Mustermann", "max.mustermann@musterdomain.com", New Date(1990, 1, 1), False))
            Console.WriteLine(" fertig.")

            Console.Write("AddOrUpdate für Posts...")
            Dim postsEdv As New List(Of Post)
            postsEdv.Add(New Post("Gruppenrichtlinien konfigurieren", "Indem man Googelt und das gelesene umsetzt! *gg*", context.Users.Local.First, context.Users.Local.ToList))
            postsEdv.Add(New Post("Windows 10 Pro installieren", "Den Anweisungen des Setups folgen bis man nicht mehr weiter weis, dann fragt man am besten in einem Forum.", context.Users.Local.First, context.Users.Local.Take(2).ToList))
            postsEdv.Add(New Post("UAC abschalten", "Am besten du lässt die Finger davon", context.Users.Local.First, context.Users.Local.Take(2).ToList))
            Console.WriteLine(" fertig.")

            Console.Write("AddOrUpdate für Blogs")
            Dim blogEdv As New Blog("Mein EDV Blog", postsEdv)
            context.Blogs.AddOrUpdate(Function(b) b.Title, blogEdv)
            Console.WriteLine(" fertig.")

            Console.Write("SaveChanges... Rückgabe: ")
            Dim contextRet As Integer = context.SaveChanges
            Console.WriteLine($"{contextRet} Datensätze.")
            If contextRet > 0 Then
                Console.WriteLine($"Es wurden erfolgreich {context.Users.Local.Count } Testuser,  {context.Blogs.Count } Blog und {context.Posts.Count } Post(s) angelegt oder upgedated.")
            Else
                Console.WriteLine("Standartwerte sind alle in DB aktuell!")
            End If
            Console.WriteLine("*** Seeding ENDE ***")
        End Sub




    End Class

End Namespace
