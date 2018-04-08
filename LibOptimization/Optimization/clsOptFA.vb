Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Firefly Algorithm
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Derivative free optimization algorithm.
    '''  -for Mulitimodal optimization
    ''' 
    ''' Refference:
    ''' [1]X. S. Yang, “Firefly algorithms for multimodal optimization,” in Proceedings of the 5th International Conference on Stochastic Algorithms: Foundation and Applications (SAGA '09), vol. 5792 of Lecture notes in Computer Science, pp. 169–178, 2009.
    ''' [2]Firefly Algorithm - http://www.mathworks.com/matlabcentral/fileexchange/29693-firefly-algorithm
    ''' 
    ''' Implment:
    ''' N.Tomi(tomi.nori+github at gmail.com)
    ''' </remarks>
    Public Class clsOptFA : Inherits absOptimization
#Region "Member"
        '----------------------------------------------------------------
        'Common parameters
        '----------------------------------------------------------------
        ''' <summary>Max iteration count(Default:5,000)</summary>
        Public Overrides Property Iteration As Integer = 5000

        ''' <summary>
        ''' epsilon(Default:1e-8) for Criterion
        ''' </summary>
        Public Property EPS As Double = 0.000000001

        ''' <summary>
        ''' Use criterion
        ''' </summary>
        Public Property IsUseCriterion As Boolean = True

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        '----------------------------------------------------------------
        'Peculiar parameter
        '----------------------------------------------------------------
        ''' <summary>
        ''' Population Size(Default:50)
        ''' </summary>
        Public Property PopulationSize As Integer = 50

        ''' <summary>
        ''' Fire Fly
        ''' </summary>
        Private m_fireflies As New List(Of clsFireFly)

        ''' <summary>
        ''' attractiveness base
        ''' </summary>
        Public Property Beta0 As Double = 1.0

        ''' <summary>
        ''' light absorption coefficient(Default:1.0)
        ''' </summary>
        Public Property Gamma As Double = 1.0

        ''' <summary>
        ''' randomization parameter
        ''' </summary>
        Public Property Alpha As Double = 0.2
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Constructor
        ''' </summary>
        ''' <param name="ai_func">Objective Function</param>
        ''' <remarks>
        ''' </remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Init
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_fireflies.Clear()
                Me.m_error.Clear()

                'initial position
                For i As Integer = 0 To Me.PopulationSize - 1
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    Me.m_fireflies.Add(New clsFireFly(MyBase.m_func, array))
                    Me.m_fireflies(i).ReEvaluate() 'with update intensity
                Next

                'Sort Evaluate
                Me.m_fireflies.Sort()

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_fireflies.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_fireflies.Count OrElse Me.HigherNPercentIndex >= Me.m_fireflies.Count Then
                    Me.HigherNPercentIndex = Me.m_fireflies.Count - 1
                End If

            Catch ex As Exception
                Me.m_error.SetError(True, clsError.ErrorType.ERR_INIT)
            End Try
        End Sub

        ''' <summary>
        ''' Do Iteration
        ''' </summary>
        ''' <param name="ai_iteration">Iteration count. When you set zero, use the default value.</param>
        ''' <returns>True:Stopping Criterion. False:Do not Stopping Criterion</returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ByVal ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'Do Iterate
            ai_iteration = If((Iteration - m_iteration) > ai_iteration, Iteration - m_iteration - 1, If((Iteration - m_iteration) > ai_iteration, ai_iteration - 1, Iteration - m_iteration - 1))
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'Sort Evaluate
                Me.m_fireflies.Sort()

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_fireflies(0).Eval, Me.m_fireflies(Me.HigherNPercentIndex).Eval) Then
                        Return True
                    End If
                End If

                'FireFly - original
                For i As Integer = 0 To Me.PopulationSize - 1
                    For j As Integer = 0 To Me.PopulationSize - 1
                        'Move firefly
                        If Me.m_fireflies(i).Intensity < Me.m_fireflies(j).Intensity Then
                            Dim r As Double = (Me.m_fireflies(i) - Me.m_fireflies(j)).NormL1()
                            Dim beta As Double = Me.Beta0 * Math.Exp(-Me.Gamma * r * r) 'attractiveness
                            For k As Integer = 0 To Me.m_func.NumberOfVariable - 1
                                Dim newPos As Double = Me.m_fireflies(i)(k)
                                newPos += beta * (Me.m_fireflies(j)(k) - Me.m_fireflies(i)(k)) 'attraction
                                newPos += Me.Alpha * (Me.m_rand.NextDouble() - 0.5) 'random search
                                Me.m_fireflies(i)(k) = newPos
                            Next k
                            Me.m_fireflies(i).ReEvaluate() 'with update intensity
                        End If
                    Next
                Next
            Next

            Return False
        End Function

        ''' <summary>
        ''' Best result
        ''' </summary>
        ''' <returns>Best point class</returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As clsPoint
            Get
                'find best index
                Dim bestIndex As Integer = 0
                Dim bestEval = Me.m_fireflies(0).Eval
                For i = 0 To Me.m_fireflies.Count - 1
                    If Me.m_fireflies(i).Eval < bestEval Then
                        bestEval = Me.m_fireflies(i).Eval
                        bestIndex = i
                    End If
                Next
                Return New clsPoint(Me.m_func, Me.m_fireflies(bestIndex).ToArray)
            End Get
        End Property

        ''' <summary>
        ''' Get recent error
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function IsRecentError() As Boolean
            Return Me.m_error.IsError()
        End Function

        ''' <summary>
        ''' All Result
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>
        ''' for Debug
        ''' </remarks>
        Public Overrides ReadOnly Property Results As List(Of clsPoint)
            Get
                Return Nothing
            End Get
        End Property
#End Region

#Region "Private"

#End Region
    End Class
End Namespace
