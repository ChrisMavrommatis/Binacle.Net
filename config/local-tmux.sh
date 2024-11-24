#!/bin/bash

SESSION_NAME="binacle"

# if session doesnt exist
if ! tmux has-session -t $SESSION_NAME 2>/dev/null; then
	# Create a new session named $SESSION_NAME with a window named 'api'
	tmux new-session -d -s $SESSION_NAME -n 'api'
	
	# split the 1st window horizontally
	tmux split-window -h -t $SESSION_NAME:0 

	# set the cd to /src/Binacle.Net.Api
	tmux send-keys -t $SESSION_NAME:0.0 'cd ./src/Binacle.Net.Api' C-m
	
	# Set the cd to ../_data
	tmux send-keys -t $SESSION_NAME:0.1 'cd ../_data' C-m
	
	# Create a new window named 'benchmarks'
	tmux new-window -n 'benchmarks' -t $SESSION_NAME
	
	# Split the 2nd window vertically
	tmux split-window -v -t $SESSION_NAME:1
	
	# Set the cd to ./test/Binacle.Net.Lib.Benchmarks
	tmux send-keys -t $SESSION_NAME:1.0 'cd ./test/Binacle.Net.Lib.Benchmarks' C-m
	tmux send-keys -t $SESSION_NAME:1.1 'cd ./test/Binacle.Net.Lib.Benchmarks' C-m
	
		# Select the first pane
  tmux select-pane -t $SESSION_NAME:1.0
	
	# Create a new window named 'performance'
	tmux new-window -n 'performance' -t $SESSION_NAME
	
	# Set the cd to ./test/Binacle.Net.Lib.PerformanceTests
	tmux send-keys -t $SESSION_NAME:2 'cd ./test/Binacle.Net.Lib.PerformanceTests' C-m
	
	# Select the first window 
	tmux select-window -t $SESSION_NAME:0
	
	# Select the first pane
	tmux select-pane -t $SESSION_NAME:0.0
fi

# Finally attach to the session
tmux attach -t $SESSION_NAME

