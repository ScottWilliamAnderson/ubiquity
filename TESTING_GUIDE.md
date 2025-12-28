# Manual Testing Guide for Intro Video Input Handling

## Overview
This guide provides step-by-step instructions for manually testing the intro video input handling implementation to verify all requirements are met.

## Prerequisites
- Playnite installed with fullscreen mode support
- Ubiquity theme installed
- Intro video file placed at one of these locations:
  - `Videos/Intro.mp4`
  - `Intro.mp4`
  - `Themes/Fullscreen/Mythic/Videos/Intro.mp4`
- Controller (Xbox, PlayStation, or compatible gamepad) for gamepad testing

## Test Scenarios

### Test 1: Video Playback
**Objective**: Verify the intro video plays automatically on startup

**Steps**:
1. Launch Playnite in fullscreen mode with Ubiquity theme
2. Observe the intro screen appears
3. Verify the video starts playing automatically
4. Verify the "Press any button to skip" text appears in bottom-right corner

**Expected Results**:
- ✅ Video loads and plays automatically
- ✅ Skip hint text is visible but not distracting (60% opacity)
- ✅ Video fills screen appropriately (Stretch="Uniform")

**Pass/Fail**: _______

---

### Test 2: Keyboard Input Blocking
**Objective**: Verify keyboard inputs don't reach Playnite during video playback

**Steps**:
1. Start Playnite with intro video
2. While video is playing, press various keys:
   - Arrow keys (Up, Down, Left, Right)
   - Enter key
   - Escape key
   - Letter keys (A, B, etc.)
3. Observe that no Playnite menus or actions are triggered

**Expected Results**:
- ✅ No game selection changes occur
- ✅ No menus open
- ✅ No games launch
- ✅ All keyboard input is blocked

**Pass/Fail**: _______

---

### Test 3: Mouse Input Blocking
**Objective**: Verify mouse clicks don't reach Playnite during video playback

**Steps**:
1. Start Playnite with intro video
2. While video is playing, try:
   - Left click in various screen areas
   - Right click
   - Middle click (if available)
   - Multiple rapid clicks
3. Observe that no Playnite actions are triggered

**Expected Results**:
- ✅ No game selections occur
- ✅ No context menus appear
- ✅ No games launch
- ✅ All mouse clicks are blocked

**Pass/Fail**: _______

---

### Test 4: Mouse Scroll Blocking
**Objective**: Verify mouse wheel scrolling doesn't reach Playnite during video playback

**Steps**:
1. Start Playnite with intro video
2. While video is playing:
   - Scroll mouse wheel up
   - Scroll mouse wheel down
   - Rapid scrolling in both directions
3. Observe that no game list scrolling occurs

**Expected Results**:
- ✅ Game list doesn't scroll
- ✅ No navigation changes occur
- ✅ All scroll input is blocked

**Pass/Fail**: _______

---

### Test 5: Gamepad Input Blocking
**Objective**: Verify gamepad/controller inputs don't reach Playnite during video playback

**Steps**:
1. Connect a gamepad/controller
2. Start Playnite with intro video
3. While video is playing, press:
   - D-pad buttons (Up, Down, Left, Right)
   - Face buttons (A, B, X, Y / Cross, Circle, Square, Triangle)
   - Shoulder buttons (LB, RB, LT, RT / L1, R1, L2, R2)
   - Start/Options button
   - Back/Select button
4. Observe that no Playnite actions are triggered

**Expected Results**:
- ✅ No game selection changes
- ✅ No menus open
- ✅ No games launch
- ✅ All gamepad input is blocked

**Pass/Fail**: _______

---

### Test 6: Skip with Keyboard
**Objective**: Verify any keyboard key skips the video immediately

**Steps**:
1. Start Playnite with intro video
2. Wait 1-2 seconds for video to start
3. Press any key (e.g., Space)
4. Observe the skip behavior

**Expected Results**:
- ✅ Video stops immediately
- ✅ Smooth 300ms fade-out animation plays
- ✅ Intro screen disappears after fade
- ✅ Playnite main interface becomes accessible
- ✅ Animation is smooth (QuadraticEase easing)

**Pass/Fail**: _______

---

### Test 7: Skip with Mouse
**Objective**: Verify any mouse click skips the video immediately

**Steps**:
1. Start Playnite with intro video
2. Wait 1-2 seconds for video to start
3. Click anywhere on screen
4. Observe the skip behavior

**Expected Results**:
- ✅ Video stops immediately
- ✅ Smooth 300ms fade-out animation plays
- ✅ Intro screen disappears after fade
- ✅ Playnite main interface becomes accessible

**Pass/Fail**: _______

---

### Test 8: Skip with Gamepad
**Objective**: Verify any gamepad button skips the video immediately

**Steps**:
1. Connect a gamepad
2. Start Playnite with intro video
3. Wait 1-2 seconds for video to start
4. Press any gamepad button (e.g., A button)
5. Observe the skip behavior

**Expected Results**:
- ✅ Video stops immediately
- ✅ Smooth 300ms fade-out animation plays
- ✅ Intro screen disappears after fade
- ✅ Playnite main interface becomes accessible

**Pass/Fail**: _______

---

### Test 9: Multiple Skip Attempts
**Objective**: Verify rapid multiple inputs don't cause issues

**Steps**:
1. Start Playnite with intro video
2. Wait 1 second for video to start
3. Rapidly press a key multiple times (e.g., Space 10 times quickly)
4. Observe the behavior

**Expected Results**:
- ✅ Only first input triggers skip
- ✅ Subsequent inputs are ignored during fade-out
- ✅ No errors or crashes occur
- ✅ Smooth single fade-out animation
- ✅ No duplicate skip attempts

**Pass/Fail**: _______

---

### Test 10: Natural Video Completion
**Objective**: Verify video ending naturally works correctly

**Steps**:
1. Start Playnite with intro video
2. Let the video play to completion without pressing anything
3. Observe the behavior when video ends

**Expected Results**:
- ✅ Video plays to the end
- ✅ Smooth fade-out animation plays when video ends
- ✅ Intro screen disappears
- ✅ Playnite main interface becomes accessible
- ✅ No errors occur

**Pass/Fail**: _______

---

### Test 11: Missing Video File
**Objective**: Verify graceful handling when video file is missing

**Steps**:
1. Remove or rename the intro video file
2. Start Playnite with intro feature enabled
3. Observe the behavior

**Expected Results**:
- ✅ No error messages displayed
- ✅ Intro screen skips gracefully
- ✅ Playnite main interface loads normally
- ✅ No crashes or freezes

**Pass/Fail**: _______

---

### Test 12: Post-Intro Input
**Objective**: Verify normal input works after intro completes

**Steps**:
1. Start Playnite with intro video
2. Skip the intro (or let it complete)
3. After intro is gone, try:
   - Navigating with keyboard
   - Clicking with mouse
   - Scrolling with mouse wheel
   - Using gamepad controls
4. Verify all inputs work normally

**Expected Results**:
- ✅ Keyboard navigation works normally
- ✅ Mouse clicks work normally
- ✅ Mouse scrolling works normally
- ✅ Gamepad controls work normally
- ✅ No lingering input blocking issues

**Pass/Fail**: _______

---

### Test 13: Animation Quality
**Objective**: Verify the fade-out animation is smooth and professional

**Steps**:
1. Start Playnite with intro video
2. Press any key to skip
3. Carefully observe the fade-out animation

**Expected Results**:
- ✅ Animation is smooth (no jerky motion)
- ✅ Duration is approximately 300ms (quick but not instant)
- ✅ Easing makes animation feel natural (not linear)
- ✅ Video stops before fade begins (no video artifacts)
- ✅ Professional appearance overall

**Pass/Fail**: _______

---

### Test 14: Skip Hint Visibility
**Objective**: Verify the skip hint text is visible but not intrusive

**Steps**:
1. Start Playnite with intro video
2. Observe the "Press any button to skip" text
3. Evaluate its visibility and intrusiveness

**Expected Results**:
- ✅ Text is readable (white on black background)
- ✅ Text is semi-transparent (60% opacity)
- ✅ Text is positioned in bottom-right corner
- ✅ Text doesn't distract from video
- ✅ Text is visible throughout video playback

**Pass/Fail**: _______

---

### Test 15: Resource Cleanup
**Objective**: Verify no memory leaks or resource issues

**Steps**:
1. Start Playnite and watch intro (or skip)
2. Use Task Manager to monitor Playnite memory usage
3. Repeat intro playback/skip several times
4. Observe memory usage over time

**Expected Results**:
- ✅ Memory usage doesn't continuously increase
- ✅ No error messages in logs
- ✅ No performance degradation
- ✅ Resources are properly cleaned up

**Pass/Fail**: _______

---

## Test Summary

**Total Tests**: 15  
**Tests Passed**: _______  
**Tests Failed**: _______  
**Pass Rate**: _______%  

---

## Notes and Observations

Use this section to record any additional observations, edge cases discovered, or suggestions for improvement:

```
[Add your notes here]
```

---

## Issues Found

If any tests fail, document the issues here:

| Test # | Issue Description | Severity | Reproduction Steps |
|--------|-------------------|----------|-------------------|
|        |                   |          |                   |

---

## Sign-off

**Tester Name**: _______________________  
**Date**: _______________________  
**Overall Assessment**: [ ] Pass  [ ] Fail  [ ] Pass with Minor Issues  

**Comments**:
```
[Add final comments here]
```
