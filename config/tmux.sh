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
	tmux send-keys -t $SESSION_NAME:0.0 'cd ./config' C-m
	
	# Set the cd to ../_data
	tmux send-keys -t $SESSION_NAME:0.1 'cd ./config' C-m
	
	# Select the first pane
	tmux select-pane -t $SESSION_NAME:0.0
	###-------------------------WINDOW 0----------------------------###
	
	###-------------------------WINDOW 1----------------------------###
  # Create a new window named 'docs'
  tmux new-window -n 'docs' -t $SESSION_NAME

	# split the 2nd window horizontally
 	tmux split-window -h -t $SESSION_NAME:1

  # set the cd to /docs
  tmux send-keys -t $SESSION_NAME:1.0 'cd ./docs' C-m

  # Set the cd to /docs
  tmux send-keys -t $SESSION_NAME:1.1 'cd ./docs' C-m

  # Select the first pane
  tmux select-pane -t $SESSION_NAME:1.0

  ###-------------------------WINDOW 1----------------------------###
  
  ###-------------------------WINDOW 2----------------------------###
  # Create a new window named 'web'
  tmux new-window -n 'web' -t $SESSION_NAME
  
	# split the window horizontally
 	tmux split-window -h -t $SESSION_NAME:2

  # set the cd to /web
  tmux send-keys -t $SESSION_NAME:2.0 'cd ./web' C-m

  # Set the cd to /web
  tmux send-keys -t $SESSION_NAME:2.1 'cd ./web' C-m

  # Select the first pane
  tmux select-pane -t $SESSION_NAME:2.0
  ###-------------------------WINDOW 2----------------------------###
	
  ###-------------------------WINDOW 3----------------------------###
  # Create a new window named 'tests'
  tmux new-window -n 'tests' -t $SESSION_NAME
  
  # set the cd to /config
  tmux send-keys -t $SESSION_NAME:3 'cd ./config' C-m
  ###-------------------------WINDOW 3----------------------------###

  ###-------------------------WINDOW 4----------------------------###
  # Create a new window named 'tests'
  tmux new-window -n 'misc' -t $SESSION_NAME
  
  # set the cd to /config
  tmux send-keys -t $SESSION_NAME:4 'cd ./config' C-m
  ###-------------------------WINDOW 4----------------------------###	
	
	
	###-------------------------WINDOW 5----------------------------###
	
	# Create a new window named 'bench_1
  tmux new-window -n 'bench_1' -t $SESSION_NAME
  	
  # Create 4 panes
  # Split the window vertically
  tmux split-window -v -t $SESSION_NAME:4
  
  # Set the cd to /config
  tmux send-keys -t $SESSION_NAME:5.0 'cd ./config' C-m
  tmux send-keys -t $SESSION_NAME:5.1 'cd ./config' C-m

  # Select the first pane
  tmux select-pane -t $SESSION_NAME:5.0
  	
	###-------------------------WINDOW 6----------------------------###
	
  ###-------------------------WINDOW 5----------------------------###
  	
  # Create a new window named 'bench_2'
  tmux new-window -n 'bench_2' -t $SESSION_NAME
    	
  # Create 4 panes
  # Split the window vertically
  tmux split-window -v -t $SESSION_NAME:6
    
  # Set the cd to /config
  tmux send-keys -t $SESSION_NAME:6.0 'cd ./config' C-m
  tmux send-keys -t $SESSION_NAME:6.1 'cd ./config' C-m
  
  # Select the first pane
  tmux select-pane -t $SESSION_NAME:6.0
    	
  ###-------------------------WINDOW 6----------------------------###
		
	###-------------------------WINDOW 7----------------------------###
  	
  	# Create a new window named 'bench_3'
    tmux new-window -n 'bench_3' -t $SESSION_NAME
    	
    # Create 4 panes
    # Split the 4th window vertically
    tmux split-window -v -t $SESSION_NAME:7
    # Split the 1st pane horizontally
    tmux split-window -h -t $SESSION_NAME:7.0
    # Split the 3rd pane horizontally
    tmux split-window -h -t $SESSION_NAME:7.2
    
    # Set the cd to /config
    tmux send-keys -t $SESSION_NAME:7.0 'cd ./config' C-m
    tmux send-keys -t $SESSION_NAME:7.1 'cd ./config' C-m
    tmux send-keys -t $SESSION_NAME:7.2 'cd ./config' C-m
    tmux send-keys -t $SESSION_NAME:7.3 'cd ./config' C-m
    # Select the first pane
    tmux select-pane -t $SESSION_NAME:7.0
    	
  ###-------------------------WINDOW 7----------------------------###
  
  # Select the first window 
  tmux select-window -t $SESSION_NAME:0
fi

# Finally attach to the session
tmux attach -t $SESSION_NAME

