Imports System.ComponentModel
Imports System.ComponentModel.DataAnnotations
Imports CodeFirst_Model.Interfaces

Public Class User
    Inherits ModelBase
    Implements ILogicalDelete
    Implements ILogicalTimestamp

    Public Sub New()
    End Sub

    Public Sub New(userName As String, mailAddress As String, Optional birthday As Date? = Nothing, Optional isActive As Boolean = True)
        Me.UserName = userName : UserMailaddress = mailAddress : Me.Birthday = birthday : Me.IsActive = isActive
    End Sub


    Public Overridable Property Id As Integer

    <Required>
    Public Overridable Property UserName As String

    <Required>
    Public Overridable Property UserMailaddress As String

    Public Overridable Property Birthday As Date?
    Public ReadOnly Property Age As Integer
        Get
            If Not Birthday.HasValue Then Return 0
            Return GetCurrentAge(Birthday.Value)
        End Get
    End Property

    <DefaultValue(True)>
    Public Overridable Property IsActive As Boolean

    Public Overridable Property OpenedPosts As ICollection(Of Post)

    Public Overridable Property MarkedInPosts As ICollection(Of Post)

    Public Property DeletedFlag As Boolean Implements ILogicalDelete.DeletedFlag
    Public Property DeletedTimestamp As Date? Implements ILogicalDelete.DeletedTimestamp
    Public Property LastUpdateTimestamp As Date? Implements ILogicalTimestamp.LastUpdateTimestamp
    Public Property CreationTimestamp As Date? Implements ILogicalTimestamp.CreationTimestamp

    Public Function GetCurrentAge(ByVal dob As Date) As Integer
        Dim tAge As Integer = DateTime.Today.Year - dob.Year
        If (dob > DateTime.Today.AddYears(-tAge)) Then tAge -= 1
        Return tAge
    End Function


    Public Overrides Function ToString() As String
        Return String.Format("User-Objekt: Username:{0} (ID: {5}), Mail: {1}, Deleted: {2}, LastChange: {3} from {4}", UserName, UserMailaddress, DeletedFlag, LastUpdateTimestamp.Value.ToLongDateString, LastUpdateBy, Id)

    End Function

End Class
