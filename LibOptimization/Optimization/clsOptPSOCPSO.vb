Imports LibOptimization.Util
Imports LibOptimization.MathUtil

Namespace Optimization
    ''' <summary>
    ''' Competitive PSOparallel competitive Particle Swarm Optimization
    ''' </summary>
    ''' <remarks>
    ''' Features:
    '''  -Swarm Intelligence algorithm.
    ''' 
    ''' Refference:
    ''' [1]Luu, Keurfon, et al. "A parallel competitive Particle Swarm Optimization for non-linear first arrival traveltime tomography and uncertainty quantification." Computers and Geosciences 113 (2018): 81-93.
    ''' </remarks>
    Public Class clsOptPSOCPSO : Inherits absOptimization
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

        ''' <summary>particles</summary>
        Private m_swarm As New List(Of clsParticle)

        ''' <summary>global best</summary>
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

        ''' <summary>Gamma</summary>
        Public Property Gamma As Double = 1.0

        ''' <summary>Delta</summary>
        Private Delta As Double
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

                'Swarm maximum radius
                Me.Delta = Math.Log(1.0 + 0.003 * m_swarm.Count) / Math.Max(0.2, Math.Log(0.01 * Iteration))

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

                'check criterion
                If Me.IsUseCriterion = True Then
                    'higher N percentage particles are finished at the time of same evaluate value.
                    If clsUtil.IsCriterion(Me.EPS, Me.m_globalBest, Me.m_swarm(Me.HigherNPercentIndex).BestPoint) Then
                        Return True
                    End If
                End If

                'PSO process
                For Each particle In Me.m_swarm
                    'replace personal best
                    If particle.Point.Eval < particle.BestPoint.Eval Then
                        particle.BestPoint = particle.Point.Copy()
                    End If

                    'update global best
                    If particle.Point.Eval < Me.m_globalBest.Eval Then
                        Me.m_globalBest = particle.Point.Copy()
                    End If
                Next
                Dim max_dist = 0.0
                For Each particle In Me.m_swarm
                    Dim temp_dist = 0.0
                    For i As Integer = 0 To Me.m_func.NumberOfVariable - 1
                        'update a velocity 
                        Dim r1 = Me.m_rand.NextDouble()
                        Dim r2 = Me.m_rand.NextDouble()
                        Dim newV = Me.Weight * particle.Velocity(i) +
                                   C1 * r1 * (particle.BestPoint(i) - particle.Point(i)) +
                                   C2 * r2 * (Me.m_globalBest(i) - particle.Point(i))
                        particle.Velocity(i) = newV

                        'update a position using velocity
                        particle.Point(i) = particle.Point(i) + particle.Velocity(i)

                        'distance
                        temp_dist += Math.Pow((particle.Point(i) - m_globalBest(i)), 2)
                        'distance
                        'temp_dist += Math.Pow((particle.Point(i) - particle.BestPoint(i)), 2)
                    Next
                    particle.Point.ReEvaluate()

                    If temp_dist > max_dist Then
                        max_dist = temp_dist
                    End If
                Next

                'restart patricles
                Dim swarm_radius = max_dist / Math.Sqrt(4.0 * m_func.NumberOfVariable())
                If swarm_radius < Me.Delta Then
                    Dim inorm = Me.m_iteration / Me.Iteration
                    inorm = 0.9
                    Dim nw = CInt(((m_swarm.Count - 1.0) / (1 + Math.Exp(1.0 / 0.09 * (inorm - Gamma + 0.5)))))
                    If nw > 0 Then
                        Me.m_swarm.Sort()
                        Dim resetSwarmSize = m_swarm.Count - nw - 1
                        For i As Integer = resetSwarmSize To m_swarm.Count - 1
                            'Particles initial positions
                            Dim tempPosition = New clsPoint(Me.m_func)
                            Dim tempBestPosition = New clsPoint(Me.m_func)
                            Dim Array = clsUtil.GenRandomPositionArray(Me.m_func, InitialPosition, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                            tempPosition = New clsPoint(Me.m_func, Array)
                            tempBestPosition = tempPosition.Copy()

                            'Initialize particle velocity
                            Dim tempVelocity = clsUtil.GenRandomPositionArray(Me.m_func, Nothing, Me.InitialValueRangeLower, Me.InitialValueRangeUpper)
                            m_swarm(i) = New clsParticle(tempPosition, tempVelocity, tempBestPosition)
                            m_swarm(i).Point.ReEvaluate()
                        Next
                    End If
                End If

                'sort by eval
                Me.m_swarm.Sort()
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
                Return clsUtil.FindGlobalBestFromParticles(m_swarm)
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
                Dim ret As New List(Of clsPoint)(Me.m_swarm.Count - 1)
                For Each p In Me.m_swarm
                    ret.Add(p.BestPoint.Copy())
                Next
                ret.Sort()
                Return ret
            End Get
        End Property
#End Region

#Region "Private"
#End Region
    End Class
End Namespace
