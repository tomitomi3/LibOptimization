Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Simulated Annealing
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Randomized algorithm for optimization.
    ''' 
    ''' Reffrence:
    ''' http://ja.wikipedia.org/wiki/%E7%84%BC%E3%81%8D%E3%81%AA%E3%81%BE%E3%81%97%E6%B3%95
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptSimulatedAnnealing : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20000)</summary>
        Public Property Iteration As Integer = 20000 'generation

        ''' <summary>Epsilon(Default:1e-8) for Criterion</summary>
        Public Property EPS As Double = 0.00000001

        '-------------------------------------------------------------------
        'Coefficient of SA(Simulated Annealing)
        '-------------------------------------------------------------------
        ''' <summary>cooling ratio</summary>
        Private CoolingRatio As Double = 0.9995 '0.1%

        ''' <summary>range of neighbor search</summary>
        Private NEIGHBOR_RANGE As Double = 0.1

        ''' <summary>start temperture</summary>
        Private TEMPERTURE As Double = 5000.0

        Private m_point As clsPoint = Nothing
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
            Me.m_point = New clsPoint(ai_func)
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                Me.m_point.Clear()
                For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                    Dim value As Double = clsUtil.GenRandomRange(Me.m_rand, -Me.InitialValueRange, Me.InitialValueRange)
                    If MyBase.InitialPosition IsNot Nothing AndAlso MyBase.InitialPosition.Length = Me.m_func.NumberOfVariable Then
                        value += Me.InitialPosition(i)
                    End If
                    Me.m_point.Add(value)
                Next
                Me.m_point.ReEvaluate()

            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do optimization
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            ai_iteration = If(ai_iteration = 0, Me.Iteration - 1, ai_iteration - 1)
            For iterate As Integer = 0 To ai_iteration
                'iteration count
                If Iteration <= Me.m_iteration Then
                    Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_OPT_MAXITERATION)
                    Return True
                End If
                Me.m_iteration += 1

                'neighbor
                Dim temp As New clsPoint(Me.m_point)
                For i As Integer = 0 To temp.Count - 1
                    temp(i) += Math.Abs(2.0 * NEIGHBOR_RANGE) * m_rand.NextDouble() - NEIGHBOR_RANGE
                Next
                temp.ReEvaluate()

                'transition
                Dim evalNow As Double = Me.m_point.Eval
                Dim evalNew As Double = temp.Eval
                Dim r1 As Double = 0.0
                Dim r2 = Me.m_rand.NextDouble()
                If evalNew < evalNow Then
                    r1 = 1.0
                Else
                    Dim delta = evalNow - evalNew
                    r1 = Math.Exp(delta / TEMPERTURE)
                End If
                If r1 >= r2 Then
                    Me.m_point = temp
                End If
                'Console.WriteLine("Random:{0:F5},{1:F5},{2:F5},{3:F5},{4:F5}", r1, r2, TEMPERTURE, delta, evalNow)

                'cooling
                If Me.TEMPERTURE > 10.0 Then
                    Me.TEMPERTURE *= Me.CoolingRatio
                End If
            Next

            Return False
        End Function

        ''' <summary>
        ''' Recent Error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As Optimization.clsPoint
            Get
                Return Me.m_point
            End Get
        End Property

        ''' <summary>
        ''' for Debug
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Results As List(Of Optimization.clsPoint)
            Get
                Return New List(Of clsPoint)({Me.m_point})
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
