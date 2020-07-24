Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Particle class for PSO
    ''' </summary>
    ''' <remarks>
    ''' for Swarm Particle Optimization
    ''' </remarks>
    Public Class clsParticle : Implements IComparable
        Private m_point As clsPoint
        Private m_bestPoint As clsPoint
        Private m_velocity As clsEasyVector

        ''' <summary>
        ''' Default construtor
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub New()
            'nop
        End Sub

        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_point"></param>
        ''' <param name="ai_velocity"></param>
        ''' <param name="ai_bestPoint"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_point As clsPoint, ByVal ai_velocity As clsEasyVector, ByVal ai_bestPoint As clsPoint)
            Me.m_point = ai_point
            Me.m_velocity = ai_velocity
            Me.m_bestPoint = ai_bestPoint
        End Sub

        ''' <summary>
        ''' Copy Constructor
        ''' </summary>
        ''' <param name="ai_particle"></param>
        ''' <remarks></remarks>
        Sub New(ByVal ai_particle As clsParticle)
            Me.m_point = ai_particle.Point.Copy()
            Me.m_velocity = ai_particle.Velocity()
            Me.m_bestPoint = ai_particle.BestPoint.Copy()
        End Sub

        ''' <summary>
        ''' Point
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Point() As clsPoint
            Get
                Return Me.m_point
            End Get
            Set(value As clsPoint)
                Me.m_point = value
            End Set
        End Property

        ''' <summary>
        ''' Velocity
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Velocity As clsEasyVector
            Get
                Return Me.m_velocity
            End Get
            Set(value As clsEasyVector)
                Me.m_velocity = value
            End Set
        End Property

        ''' <summary>
        ''' BestPoint
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property BestPoint As clsPoint
            Get
                Return Me.m_bestPoint
            End Get
            Set(value As clsPoint)
                Me.m_bestPoint = value
            End Set
        End Property

        ''' <summary>
        ''' for sort
        ''' </summary>
        ''' <param name="ai_obj"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CompareTo(ByVal ai_obj As Object) As Integer Implements IComparable.CompareTo
            'Nothing check
            If ai_obj Is Nothing Then
                Return 1
            End If

            'Type check
            If Not Me.GetType() Is ai_obj.GetType() Then
                Throw New ArgumentException("Different type", "obj")
            End If

            'Compare
            Dim mineValue As Double = Me.m_bestPoint.Eval
            Dim compareValue As Double = DirectCast(ai_obj, clsParticle).BestPoint.Eval
            If mineValue < compareValue Then
                Return -1
            ElseIf mineValue > compareValue Then
                Return 1
            Else
                Return 0
            End If
        End Function
    End Class
End Namespace
