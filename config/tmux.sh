#!/bin/bash

SESSION_NAME="binacle"

# if session doesnt exist
if ! tmux has-session -t $SESSION_NAME 2>/dev/null; then
	# Create a new session named $SESSION_NAME with a window named 'api'
	tmux new-session -d -s $SESSION_NAME -n 'api'
	
	###-------------------------WINDOW 0----------------------------###
	# split the 1st window horizontally
	tmux split-window -h -t $SESSION_NAME:0 

	# set the cd to /src/Binacle.Net
	tmux send-keys -t $SESSION_NAME:0.0 'cd ./src/Binacle.Net' C-m
	
	# Set the cd to ../_data
	tmux send-keys -t $SESSION_NAME:0.1 'cd ./config/aspire-dashboard-otel' C-m
	
	# Select the first pane
	tmux select-pane -t $SESSION_NAME:0.0
	###-------------------------WINDOW 0----------------------------###
	
	###-------------------------WINDOW 1----------------------------###
	# Create a new window named 'build'
	tmux new-window -n 'build' -t $SESSION_NAME
	
	# set the cd to /build/local
	tmux send-keys -t $SESSION_NAME:1 'cd ./build/local' C-m
	###-------------------------WINDOW 1----------------------------###
	
	###-------------------------WINDOW 2----------------------------###
	
	# Create a new window named 'benchmarks_cube_scaling'
  tmux new-window -n 'benchmarks_cube_scaling' -t $SESSION_NAME
  	
  # Create 4 panes
  # Split the 3rd window vertically
  tmux split-window -v -t $SESSION_NAME:2
  
  # Set the cd to /config/benchmarks
  tmux send-keys -t $SESSION_NAME:2.0 'cd ./config/benchmarks' C-m
  tmux send-keys -t $SESSION_NAME:2.1 'cd ./config/benchmarks' C-m

  # Select the first pane
  tmux select-pane -t $SESSION_NAME:2.0
  	
	###-------------------------WINDOW 2----------------------------###
	
  ###-------------------------WINDOW 3----------------------------###
  	
  # Create a new window named 'benchmarks_multiple_bins'
  tmux new-window -n 'benchmarks_multiple_bins' -t $SESSION_NAME
    	
  # Create 4 panes
  # Split the 3rd window vertically
  tmux split-window -v -t $SESSION_NAME:3
    
  # Set the cd to /config/benchmarks
  tmux send-keys -t $SESSION_NAME:3.0 'cd ./config/benchmarks' C-m
  tmux send-keys -t $SESSION_NAME:3.1 'cd ./config/benchmarks' C-m
  
  # Select the first pane
  tmux select-pane -t $SESSION_NAME:3.0
    	
  ###-------------------------WINDOW 3----------------------------###
		
	###-------------------------WINDOW 4----------------------------###
  	
  	# Create a new window named 'bench'
    tmux new-window -n 'bench' -t $SESSION_NAME
    	
    # Create 4 panes
    # Split the 4th window vertically
    tmux split-window -v -t $SESSION_NAME:4
    # Split the 1st pane horizontally
    tmux split-window -h -t $SESSION_NAME:4.0
    # Split the 3rd pane horizontally
    tmux split-window -h -t $SESSION_NAME:4.2
    
    # Set the cd to /config/benchmarks
    tmux send-keys -t $SESSION_NAME:4.0 'cd ./config/benchmarks' C-m
    tmux send-keys -t $SESSION_NAME:4.1 'cd ./config/benchmarks' C-m
    tmux send-keys -t $SESSION_NAME:4.2 'cd ./config/benchmarks' C-m
    tmux send-keys -t $SESSION_NAME:4.3 'cd ./config/benchmarks' C-m
    # Select the first pane
    tmux select-pane -t $SESSION_NAME:4.0
    	
  ###-------------------------WINDOW 3----------------------------###
  
  # Select the first window 
  tmux select-window -t $SESSION_NAME:0
fi

# Finally attach to the session
tmux attach -t $SESSION_NAME

