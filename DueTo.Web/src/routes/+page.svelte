
<script lang="ts">
    import TaskList from '../components/TaskList.svelte';
    import type {TaskDto} from "../components/TaskCard.svelte";

    const tasks: TaskDto[] = [
        {
            id: '1',
            text: 'Do something',
            color: '#22d3ee',
            type: 'Development',
            priority: 'high', // ✅ OK
            isDone: false,
            activeDays: [
                { id: 1, label: 'Mon' },
                { id: 2, label: 'Tue' }
            ]
        }
    ];

</script>

<TaskList {tasks} />
