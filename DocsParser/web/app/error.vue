<!-- eslint-disable vue/multi-word-component-names -->
<script setup lang="ts">
import type { NuxtError } from '#app';

defineProps<{ error: NuxtError }>();

const router = useRouter();

function goHome() {
  void clearError({ redirect: '/' });
}

function goBack() {
  void clearError();
  router.back();
}
</script>

<template>
  <UApp>
    <NuxtLoadingIndicator
      color="var(--ui-primary)"
      :height="2"
    />
    <LayoutHeader />
    <UMain class="lg:pt-[calc(var(--ui-header-height))]">
      <UError
        icon="i-lucide-file-x"
        :error="error"
        :clear="false"
      >
        <template #links>
          <UButton
            label="Go back"
            icon="i-lucide-arrow-left"
            color="neutral"
            variant="subtle"
            size="lg"
            @click="goBack"
          />
          <UButton
            label="Back to home"
            icon="i-lucide-home"
            size="lg"
            @click="goHome"
          />
        </template>
      </UError>
    </UMain>
    <LayoutFooter />
  </UApp>
</template>