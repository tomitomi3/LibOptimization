Imports LibOptimization.Util
Imports LibOptimization.MathTool

Namespace Optimization
    ''' <summary>
    ''' Basic Particle Swarm Optmization
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    '''  -Derivative free optimization algorithm.
    ''' 
    ''' Refference:
    ''' [1]James Kennedy and Russell Eberhart, "Particle Swarm Optimization", Proceedings of IEEE the International Conference on Neural Networks，1995
    ''' [2]Eberhart, Russell, and James Kennedy. "A new optimizer using particle swarm theory." MHS'95. Proceedings of the Sixth International Symposium on Micro Machine and Human Science. Ieee, 1995.
    ''' [3]Y. Shi and Russell Eberhart, "A Modified Particle Swarm Optimizer", Proceedings of Congress on Evolu-tionary Computation, 79-73., 1998
    ''' [4]R. C. Eberhart and Y. Shi, "Comparing inertia weights and constriction factors in particle swarm optimization", In Proceedings of the Congress on Evolutionary Computation, vol. 1, pp. 84–88, IEEE, La Jolla, Calif, USA, July 2000.
    ''' </remarks>
    <Serializable>
    Public Class clsOptPSO : Inherits absOptimization
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

        ''' <summary>Inertia weight. Weigth=1.0(orignal paper 1995), Weight=0.729(Default setting)</summary>
        Public Property Weight As Double = 0.729

        ''' <summary>velocity coefficient(affected by personal best). C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)</summary>
        Public Property C1 As Double = 1.49445

        ''' <summary>velocity coefficient(affected by global best). C1 = C2 = 2.0 (orignal paper 1995), C1 = C2 = 1.49445(Default setting)</summary>
        Public Property C2 As Double = 1.49445

        ''' <summary>Neighborhood coefficient for LocalBest. compares its error value with particle(i-1) and particle(i+1).</summary>
        Public Property Neighborhood As Integer = 6

        ''' <summary>SwarmType[1][2]</summary>
        Public Enum EnumSwarmType
            ''' <summary>Global best(original pso)[1]</summary>
            GlobalBest
            ''' <summary>Local best[2]</summary>
            LocalBest
        End Enum

        ''' <summary>SwarmType default:GlobalBest</summary>
        Public Property SwarmType As EnumSwarmType = EnumSwarmType.GlobalBest

        ''' <summary>Zero Velocity Initialization(Engelbrecht 2012). default:true.</summary>
        Public Property IsUseZeroVelocityInitialization As Boolean = True
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
                    Dim tempVelocity = New Double(Me.m_func.NumberOfVariable - 1) {}
                    If IsUseZeroVelocityInitialization = False Then
                        tempVelocity = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                    End If

                    'create
                    Me.m_swarm.Add(New clsParticle(tempPosition, tempVelocity, tempBestPosition))
                Next

                'Sort Evaluate
                Me.m_swarm.Sort()
                Me.m_globalBest = Me.m_swarm(0).BestPoint.Copy()

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
                'common process
                For Each particle In Me.m_swarm
                    'replace personal best, find global best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()
                    End If

                    'find globalbest
                    If particle.BestPoint.Eval < Me.m_globalBest.Eval Then
                        Me.m_globalBest = particle.BestPoint
                    End If
                Next
                Me.m_globalBest = Me.m_globalBest.Copy()

                If Me.SwarmType = EnumSwarmType.GlobalBest Then
                    'global best. particles move toward the global best.

                    'update velocity
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

                    'sort by eval.
                    'Me.m_swarm.Sort()
                ElseIf Me.SwarmType = EnumSwarmType.LocalBest Then
                    'local best.
                    Dim halfNeighbor = CInt(Me.Neighborhood / 2)
                    For pp As Integer = 0 To Me.m_swarm.Count - 1
                        Dim lBests = New List(Of clsParticle)

                        'first half p[i-n] p[i-1] p[i-2]
                        Dim startIndex = pp - halfNeighbor
                        For i As Integer = startIndex To startIndex + halfNeighbor - 1
                            If i < 0 Then
                                Exit For
                            End If
                            lBests.Add(Me.m_swarm(i))
                        Next

                        'second half  p[i+n] p[i+1] p[i+2]
                        Dim endIndex = pp + halfNeighbor - 1
                        For i As Integer = pp To endIndex
                            If i >= Me.m_swarm.Count Then
                                Exit For
                            End If
                            lBests.Add(Me.m_swarm(i))
                        Next
                        Dim lBest = lBests(clsUtil.FindCurrentBestIndexFromParticles(lBests)).BestPoint().Copy()

                        'update a velocity 
                        Dim particle = Me.m_swarm(pp)
                        For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                            Dim r1 = Me.m_rand.NextDouble()
                            Dim r2 = Me.m_rand.NextDouble()
                            Dim newV = Me.Weight * particle.Velocity(i) +
                                       C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) +
                                       C2 * r2 * (lBest(i) - particle.Point(i))
                            particle.Velocity(i) = newV

                            'update a position using velocity
                            Dim newPos = particle.Point(i) + particle.Velocity(i)
                            particle.Point(i) = newPos
                        Next
                        particle.Point.ReEvaluate()
                    Next

                    'sort by eval. not use local best PSO
                    'Me.m_swarm.Sort()
                End If
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
