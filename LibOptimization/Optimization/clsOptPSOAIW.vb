Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Particle Swarm Optmization using Adaptive Inertia Weight(AIW-PSO)
    ''' AdaptW
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]A. Nickabadi, M. M. Ebadzadeh, and R. Safabakhsh, “A novel particle swarm optimization algorithm with adaptive inertia weight,” Applied Soft Computing Journal, vol. 11, no. 4, pp. 3658–3670, 2011.
    ''' </remarks>
    <Serializable>
    Public Class clsOptPSOAIW : Inherits absOptimization
#Region "Member"
        ''' <summary>Max iteration count(Default:20,000)</summary>
        Public Overrides Property Iteration As Integer = 20000

        ''' <summary>Epsilon(Default:0.000001) for Criterion</summary>
        Public Property EPS As Double = 0.000001 '1e-6

        ''' <summary>
        ''' higher N percentage particles are finished at the time of same evaluate value.
        ''' This parameter is valid is when IsUseCriterion is true.
        ''' </summary>
        Public Property HigherNPercent As Double = 0.8 'for IsCriterion()
        Private HigherNPercentIndex As Integer = 0 'for IsCriterion())

        'particles
        Private m_swarm As New List(Of clsParticle)
        Private m_globalBest As clsPoint = Nothing

        '-------------------------------------------------------------------
        'Coefficient of PSO
        '-------------------------------------------------------------------
        ''' <summary>Swarm Size(Default:100)</summary>
        Public Property SwarmSize As Integer = 100

        ''' <summary>adaptive inertia weight(Default:1.0)</summary>
        Public Property Weight As Double = 1.0

        ''' <summary>Weight max for adaptive weight(Default:1.0).</summary>
        Public Property WeightMax As Double = 1.0

        ''' <summary>Weight min for adaptive weight(Default:0.0).</summary>
        Public Property WeightMin As Double = 0.0

        ''' <summary>velocity coefficient(affected by personal best)(Default:1.49445)</summary>
        Public Property C1 As Double = 1.49445

        ''' <summary>velocity coefficient(affected by global best)(Default:1.49445)</summary>
        Public Property C2 As Double = 1.49445
#End Region

#Region "Constructor"
        ''' <summary>
        ''' Default constructor
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New(ByVal ai_func As absObjectiveFunction)
            Me.m_func = ai_func
        End Sub
#End Region

#Region "Public"
        ''' <summary>
        ''' Initialize
        ''' </summary>
        ''' <remarks></remarks>
        Public Overrides Sub Init()
            Try
                'init meber varibles
                Me.m_iteration = 0
                Me.m_swarm.Clear()

                'init position
                For i As Integer = 0 To Me.SwarmSize - 1
                    'position
                    Dim tempPosition = New clsPoint(Me.m_func)
                    Dim tempBestPosition = New clsPoint(Me.m_func)
                    Dim array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    tempPosition = New clsPoint(Me.m_func, array)
                    tempBestPosition = tempPosition.Copy()

                    'velocity
                    Dim tempVelocity = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)

                    'create swarm
                    Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                Next

                'Sort Evaluate
                Me.m_swarm.Sort()
                Me.m_globalBest = Me.m_swarm(0).BestPoint.Copy()
                Me.Weight = 1

                'Detect HigherNPercentIndex
                Me.HigherNPercentIndex = CInt(Me.m_swarm.Count * Me.HigherNPercent)
                If Me.HigherNPercentIndex = Me.m_swarm.Count OrElse Me.HigherNPercentIndex >= Me.m_swarm.Count Then
                    Me.HigherNPercentIndex = Me.m_swarm.Count - 1
                End If

            Catch ex As Exception
                Me.m_error.SetError(True, Util.clsError.ErrorType.ERR_INIT)
            Finally
                System.GC.Collect()
            End Try
        End Sub

        ''' <summary>
        ''' Do optimize
        ''' </summary>
        ''' <param name="ai_iteration"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function DoIteration(Optional ai_iteration As Integer = 0) As Boolean
            'Check Last Error
            If Me.IsRecentError() = True Then
                Return True
            End If

            'do iterate
            If Me.Iteration <= m_iteration Then
                Return True
            Else
                ai_iteration = If(ai_iteration = 0, Iteration - m_iteration - 1, Math.Min(ai_iteration, Iteration - m_iteration) - 1)
            End If
            For iterate As Integer = 0 To ai_iteration
                'Counting generation
                m_iteration += 1

                'check criterion - higher N percentage particles are finished at the time of same evaluate value.
                If Me.IsUseCriterion = True AndAlso clsUtil.IsCriterion(Me.EPS, Me.m_swarm, Me.HigherNPercentIndex) Then
                    Return True
                End If

                'PSO process
                'update a velocity 
                For Each particle In Me.m_swarm
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) +
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) +
                                   C2 * r2 * (Me.m_globalBest(i) - particle.Point(i))
                        particle.Velocity(i) = newV

                        'update a position using velocity
                        Dim newPos = particle.Point(i) + particle.Velocity(i)
                        particle.Point(i) = newPos
                    Next
                    particle.Point.ReEvaluate()
                Next

                Dim replaceBestCount As Integer = 0
                For Each particle In Me.m_swarm
                    'replace personal best, find global best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()
                        replaceBestCount += 1 'for AIWPSO
                    End If

                    'find globalbest
                    If particle.BestPoint.Eval < Me.m_globalBest.Eval Then
                        Me.m_globalBest = particle.BestPoint
                    End If
                Next
                Me.m_globalBest = Me.m_globalBest.Copy()

                'Inertia Weight Strategie - AIW Adaptive Inertia Weight
                Dim PS = replaceBestCount / Me.SwarmSize
                Me.Weight = (Me.WeightMax - Me.WeightMin) * PS - Me.WeightMin

                'sort by eval
                'Me.m_swarm.Sort()
            Next

            Return False
        End Function

        ''' <summary>
        ''' Result (return global best)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides ReadOnly Property Result As Optimization.clsPoint
            Get
                'find best index
                'Dim bestIndex As Integer = 0
                'Dim bestEval = Me.m_swarm(0).BestPoint.Eval
                'For i = 0 To Me.m_swarm.Count - 1
                '    If Me.m_swarm(i).BestPoint.Eval < bestEval Then
                '        bestEval = Me.m_swarm(i).BestPoint.Eval
                '        bestIndex = i
                '    End If
                'Next
                'Return Me.m_swarm(0).BestPoint.Copy()
                Return m_swarm(clsUtil.FindCurrentBestIndexFromParticles(m_swarm)).BestPoint.Copy()
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
                Me.m_swarm.Sort()
                Dim ret As New List(Of clsPoint)(Me.m_swarm.Count - 1)
                For Each p In Me.m_swarm
                    ret.Add(p.BestPoint.Copy())
                Next
                Return ret
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
