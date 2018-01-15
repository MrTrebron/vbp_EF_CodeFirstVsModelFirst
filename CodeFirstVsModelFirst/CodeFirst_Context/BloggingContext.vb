Imports System.Data.Entity
Imports CodeFirst_Model
Imports CodeFirst_Model.Interfaces

Public Class BloggingContext
    Inherits DbContext

    Public Overridable Property Blogs As DbSet(Of Blog)
    Public Overridable Property Posts As DbSet(Of Post)
    Public Overridable Property Users As DbSet(Of User)

    Public Sub New()
        Database.SetInitializer(New MigrateDatabaseToLatestVersion(Of BloggingContext, Migrations.Configuration))
        'Database.Log = Sub(s) LogEFToConsole(s)
    End Sub
    Public Sub New(Optional withLogging As Boolean = True)
        Database.SetInitializer(New MigrateDatabaseToLatestVersion(Of BloggingContext, Migrations.Configuration))
        If withLogging Then Database.Log = Sub(s) LogEFToConsole(s)
    End Sub
    Protected Overrides Sub OnModelCreating(modelBuilder As DbModelBuilder)
        MyBase.OnModelCreating(modelBuilder)

        modelBuilder.Entity(Of Post).HasMany(Function(p) p.MarkedUsers).WithMany(Function(u) u.MarkedInPosts)
    End Sub


    Public Overrides Function SaveChanges() As Integer
        Dim objectStateEntries = ChangeTracker.Entries().Where(Function(e) TypeOf e.Entity Is ModelBase AndAlso e.State <> EntityState.Detached AndAlso e.State <> EntityState.Unchanged).ToList()
        For Each _Entry In objectStateEntries
            Dim entityBase = TryCast(_Entry.Entity, ModelBase)
            If entityBase Is Nothing Then Continue For
            Select Case _Entry.State
                Case EntityState.Deleted
                    Dim logicalDelete = TryCast(entityBase, ILogicalDelete)
                    If (logicalDelete IsNot Nothing) Then
                        _Entry.State = EntityState.Modified
                        logicalDelete.DeletedTimestamp = DateTime.Now
                        logicalDelete.DeletedFlag = True
                    End If
                Case EntityState.Modified
                    Dim logicalTimestamp = TryCast(entityBase, ILogicalTimestamp)
                    If (logicalTimestamp IsNot Nothing) Then
                        logicalTimestamp.LastUpdateTimestamp = DateTime.Now
                    End If

                Case EntityState.Added
                    Dim logicalTimestamp = TryCast(entityBase, ILogicalTimestamp)
                    If (logicalTimestamp IsNot Nothing) Then
                        logicalTimestamp.CreationTimestamp = DateTime.Now
                        logicalTimestamp.LastUpdateTimestamp = DateTime.Now
                    End If
            End Select
        Next
        Return MyBase.SaveChanges()
    End Function


    Private Sub LogEFToConsole(s As String)
        Dim oldColor As ConsoleColor = Console.ForegroundColor
        Console.ForegroundColor = ConsoleColor.DarkYellow
        Console.WriteLine(String.Empty)
        Console.WriteLine(s)
        Console.WriteLine(String.Empty)
        Console.ForegroundColor = oldColor
    End Sub

End Class
